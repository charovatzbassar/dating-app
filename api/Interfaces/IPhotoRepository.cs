using System;
using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IPhotoRepository
{
    Task<List<PhotoForApprovalDTO>> GetUnapprovedPhotos(string username);
    Task<PhotoForApprovalDTO?> GetPhotoById(int photoId);
    void RemovePhoto(Photo photo);
}
