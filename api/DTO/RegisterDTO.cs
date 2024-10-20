using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTO;

public class RegisterDTO
{
    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public required string Password { get; set; }
}
