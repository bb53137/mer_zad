using System.ComponentModel.DataAnnotations;

namespace CustomerApi.DTOs;

public class CreateCustomerDto
{
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;
}