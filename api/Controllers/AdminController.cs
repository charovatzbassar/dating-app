using System;
using api.Entities;
using api.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager
            .Users
            .OrderBy(x => x.UserName)
            .Select(x => new { x.Id, Username = x.UserName, Roles = x.UserRoles.Select(r => r.Role.Name).ToList() })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("No roles selected.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("No user found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles.");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-approve")]
    public async Task<ActionResult<List<Photo>>> GetPhotosForApproval()
    {
        var photosForApproval = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();

        if (photosForApproval == null) return BadRequest("There are no photos to approve.");

        return Ok(photosForApproval);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("moderate-photo/{username}/{photoId}")]
    public async Task<ActionResult<bool>> ModeratePhoto(string username, int photoId, [FromQuery] string action)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("No user found");

        var photo = user.Photos.Find(x => x.Id == photoId);

        if (photo == null) return BadRequest("Photo not found");

        switch (action)
        {
            case "APPROVE":
                photo.IsApproved = true;
                if (user.Photos.Count == 1) photo.IsMain = true;
                break;
            case "REJECT":
                photo.IsApproved = false;
                break;
            default:
                return BadRequest("This action is not supported.");
        }

        return Ok(await unitOfWork.Complete());
    }
}
