using System;
using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class PhotoRepository(DataContext context, IMapper mapper) : IPhotoRepository
{
    public async Task<PhotoForApprovalDTO?> GetPhotoById(int photoId)
    {
        return await context.Photos
        .Where(x => x.Id == photoId)
        .ProjectTo<PhotoForApprovalDTO>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<List<PhotoForApprovalDTO>> GetUnapprovedPhotos()
    {
        return await context.Photos
            .Where(x => x.IsApproved == null)
            .ProjectTo<PhotoForApprovalDTO>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
