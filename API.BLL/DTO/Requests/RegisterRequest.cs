using System.ComponentModel.DataAnnotations;

namespace API.BLL.DTO.Request
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Lastname { set; get; } = string.Empty;
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Firstname { set; get; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { set; get; } = string.Empty;
        [Required]
        [StringLength(18, MinimumLength = 8)]
        public string Password { set; get; } = string.Empty;
    }
}
