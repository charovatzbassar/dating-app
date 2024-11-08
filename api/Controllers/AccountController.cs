using System;
using System.Security.Cryptography;
using System.Text;
using api.Data;
using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username))
        {
            return BadRequest("Username is taken");
        }


        using var hmac = new HMACSHA512(); // using statement ensures that the object is disposed of when it goes out of scope

        var user = mapper.Map<AppUser>(registerDTO);

        user.Username = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        });
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        if (!await UserExists(loginDTO.Username))
        {
            return Unauthorized("Invalid username");
        }

        var user = await context.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x =>
                x.Username == loginDTO.Username.ToLower()
            );

        if (user == null)
        {
            return Unauthorized("Invalid username");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }

        return Ok(new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs
        });
    }


    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()); // EF does not like equals method
    }
}
