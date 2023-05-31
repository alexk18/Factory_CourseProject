using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.BLL.Services.JWT.Settings
{
    public class JwtSettings
    {
        public const string ISSUER = "TestProject";
        public const string AUDIENCE = "TestProjectClient";
        private static readonly string KEY = RandomKey.CreateKey(30);
        public const int LIFETIME = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
