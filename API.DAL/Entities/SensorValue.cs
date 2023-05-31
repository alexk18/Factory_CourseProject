namespace API.DAL.Entities;

public class SensorValue
{
    public Guid Id { get; set; }
    public double Value { get; set; }

    public Guid SensorId { get; set; }
    public Sensor? Sensor { get; set; }

    public DateTime Created { get; set; }
}
