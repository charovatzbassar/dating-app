using System;
using System.ComponentModel.DataAnnotations;

namespace api.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
}
