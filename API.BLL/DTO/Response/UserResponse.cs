using API.DAL.Entities;

namespace API.BLL.DTO.Response
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Lastname { set; get; } = string.Empty;
        public string Firstname { set; get; } = string.Empty;
        public string Email { set; get; } = string.Empty;
        public Role Role { get; set; }
    }
}
