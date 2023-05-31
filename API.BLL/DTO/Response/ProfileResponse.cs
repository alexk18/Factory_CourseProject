namespace API.BLL.DTO.Response
{
    public class ProfileResponse
    {
        public string Lastname { set; get; } = string.Empty;
        public string Firstname { set; get; } = string.Empty;
        public string Email { set; get; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
