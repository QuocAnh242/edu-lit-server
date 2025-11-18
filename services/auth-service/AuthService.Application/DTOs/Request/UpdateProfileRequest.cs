using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs.Request
{
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string FullName { get; set; } = null!;
    }
}

