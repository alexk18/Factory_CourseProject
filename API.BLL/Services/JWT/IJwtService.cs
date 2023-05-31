using API.BLL.Services.JWT.Models;

namespace API.BLL.Services.JWT
{
    public interface IJwtService
    {
        public string GetToken(JwtUser user);
    }
}
