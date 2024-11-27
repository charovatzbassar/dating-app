using System;
using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId, int likedUserId);
    Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}
