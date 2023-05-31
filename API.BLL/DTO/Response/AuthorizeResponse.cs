namespace API.BLL.DTO.Response
{
    public class AuthorizeResponse
    {
        public string Token { set; get; } = string.Empty;
        public string Role { set; get; } = string.Empty;
        public Guid UserId { set; get; }
    }
}
