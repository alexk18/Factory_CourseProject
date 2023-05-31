namespace API.DAL.Entities;

public class Sensor
{
    public Guid Id { get; set; }
    public string ValueType { get; set; } = string.Empty;
    public double Min { get; set; }
    public double Max { get; set; }

    public Guid RoomId { get; set; }
    public Room? Room { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<SensorValue> Values { get; set; } = new List<SensorValue>();
}
