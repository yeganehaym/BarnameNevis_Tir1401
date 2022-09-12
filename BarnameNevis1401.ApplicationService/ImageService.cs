﻿using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;

public class ImageService:IImageService
{
    private ApplicationDbContext _context;

    public ImageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> GetSumOfBytes(int userId)
    {
        return await _context
            .ImageItems
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.FileSize);
    }
}