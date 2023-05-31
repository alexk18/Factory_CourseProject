namespace API.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Lastname { set; get; } = string.Empty;
        public string Firstname { set; get; } = string.Empty;
        public string Email { set; get; } = string.Empty;
        public string Password { set; get; } = string.Empty;
        public Role Role { get; set; } = Role.Worker;

        public List<Visit> Visits { get; set; } = new List<Visit>();
    }
}
