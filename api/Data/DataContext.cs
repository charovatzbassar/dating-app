using System;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
