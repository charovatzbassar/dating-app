using System;
using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
    Task<MemberDTO?> GetMemberAsync(string username, string currentUsername);
}
