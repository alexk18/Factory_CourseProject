namespace API.DAL.Entities;

public class Room
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsVisible { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<Sensor> Sensors { get; set; } = new List<Sensor>();
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Visit> Visits { get; set; } = new List<Visit>();
}