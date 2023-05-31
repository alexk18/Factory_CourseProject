using API.DAL.Entities;

namespace API.BLL.Services.JWT.Models
{
    public class JwtUser
    {
        public Guid Id { set; get; }
        public Role Role { set; get; }
    }
}
