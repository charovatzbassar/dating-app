using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTO;

public class RegisterDTO
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;
}
