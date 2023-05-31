using System.ComponentModel.DataAnnotations;

namespace API.BLL.DTO.Request
{
    public class AuthenticateRequest
    {
        [Required]
        [EmailAddress]
        public string Email { set; get; } = string.Empty;
        [Required]
        [StringLength(18, MinimumLength = 8)]
        public string Password { set; get; } = string.Empty;
    }
}
