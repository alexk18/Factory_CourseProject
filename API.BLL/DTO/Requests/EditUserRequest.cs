using API.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.BLL.DTO.Request
{
    public class EditUserRequest
    {
        public Guid UserId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Lastname { set; get; } = "";
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Firstname { set; get; } = "";
        public Role Role { set; get; }
    }
}
