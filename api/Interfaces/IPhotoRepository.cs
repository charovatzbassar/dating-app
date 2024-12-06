using System;
using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IPhotoRepository
{
    Task<List<PhotoForApprovalDTO>> GetUnapprovedPhotos();
    Task<PhotoForApprovalDTO?> GetPhotoById(int photoId);
    void RemovePhoto(Photo photo);
}
