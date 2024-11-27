using System;
using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IPhotoRepository
{
    Task<List<Photo>> GetUnapprovedPhotos(string username);
    Task<Photo?> GetPhotoById(int photoId);
    void RemovePhoto(Photo photo);
}
