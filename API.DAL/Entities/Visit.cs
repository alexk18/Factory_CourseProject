namespace API.DAL.Entities;

public class Visit
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
}
