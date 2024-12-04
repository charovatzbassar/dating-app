using System;
using System.Diagnostics;
using api.DTO;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<MemberDTO?> GetMemberAsync(string username, string currentUsername)
    {
        var query = context.Users.AsQueryable();
        Console.WriteLine(currentUsername + " " + username);
        if (username != currentUsername && currentUsername != "admin")
        {
            query = GetFilteredUsers(query);
        }

        return await query
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();

        query = GetFilteredUsers(query);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query
            .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob)
            .Where(x => x.UserName != userParams.CurrentUsername);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users
        .Include(p => p.Photos)
        .ToListAsync();
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    private IQueryable<AppUser> GetFilteredUsers(IQueryable<AppUser> query)
{
    return query
        .Select(x => new AppUser
        {
            UserName = x.UserName,
            DateOfBirth = x.DateOfBirth,
            Interests = x.Interests,
            KnownAs = x.KnownAs,
            City = x.City,
            Country = x.Country,
            PasswordHash = x.PasswordHash,
            PhoneNumber = x.PhoneNumber,
            AccessFailedCount = x.AccessFailedCount,
            LastActive = x.LastActive,
            Email = x.Email,
            ConcurrencyStamp = x.ConcurrencyStamp,
            Created = x.Created,
            EmailConfirmed = x.EmailConfirmed,
            Introduction = x.Introduction,
            LockoutEnabled = x.LockoutEnabled,
            LockoutEnd = x.LockoutEnd,
            LookingFor = x.LookingFor,
            NormalizedEmail = x.NormalizedEmail,
            NormalizedUserName = x.NormalizedUserName,
            PhoneNumberConfirmed = x.PhoneNumberConfirmed,
            SecurityStamp = x.SecurityStamp,
            TwoFactorEnabled = x.TwoFactorEnabled,
            Gender = x.Gender,
            Photos = x.Photos.Where(p => p.IsApproved == true).ToList()
        });
}
}
