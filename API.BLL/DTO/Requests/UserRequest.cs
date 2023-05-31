using API.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.BLL.DTO.Request
{
    public class UserRequest
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
        public string Password { set; get; } = string.Empty;
        public Role Role { set; get; }
    }
}
