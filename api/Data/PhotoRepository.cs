using System;
using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int photoId)
    {
        return await context.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
    }

    public async Task<List<Photo>> GetUnapprovedPhotos(string username)
    {
        return await context.Photos.Where(x => !x.IsApproved).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
