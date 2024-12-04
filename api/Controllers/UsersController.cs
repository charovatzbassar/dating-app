using System;
using System.Security.Claims;
using api.Data;
using api.DTO;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class UsersController(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPhotoService photoService
) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await unitOfWork.UserRepository.GetMemberAsync(username, User.GetUsername());

        if (user == null) return NotFound();

        return Ok(user);
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {

        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        mapper.Map(memberUpdateDTO, user);

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            IsApproved = null,
        };

        user.Photos.Add(photo);

        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetUser), new
            {
                username = user.UserName
            }, mapper.Map<PhotoDTO>(photo));
            // Better to return 201 
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain || photo.IsApproved == null || photo.IsApproved == false) return BadRequest("This is your main photo, or it does not exist");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to delete photo");
    }
}
