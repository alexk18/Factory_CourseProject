using System.Diagnostics;
using System.Text;
using System.Text.Json;
using API.DAL.EF;
using API.DAL.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace API.WorkerServices;

public class SensorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan interval = new(0, 5, 0);

    public SensorWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var _scope = _scopeFactory.CreateScope())
            {
                var _applicationContext = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var sensors = await _applicationContext.Sensors.ToListAsync();

                foreach (var sensor in sensors)
                {
                    var sensorValue = new SensorValue { Value = new Random().NextDouble(sensor.Min, sensor.Max), Created = DateTime.UtcNow };

                    sensor.Values.Add(sensorValue);

                }
                await _applicationContext.SaveChangesAsync(stoppingToken);
            }
            await Task.Delay(interval, stoppingToken);
        }
    }
}

//public class SensorWorker : BackgroundService
//{
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly TimeSpan interval = new(0, 5, 0);
//    private IConnection _connection;
//    private IModel _channel;
//    public SensorWorker(IServiceScopeFactory scopeFactory)
//    {
//        var factory = new ConnectionFactory { HostName = "localhost" };
//        _connection = factory.CreateConnection();
//        _channel = _connection.CreateModel();
//        _channel.QueueDeclare(queue: "MyQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
//    }

//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        stoppingToken.ThrowIfCancellationRequested();

//        var consumer = new EventingBasicConsumer(_channel);
//        consumer.Received += (ch, ea) =>
//        {

//           var content = Encoding.UTF8.GetString(ea.Body.ToArray());

//                var result = JsonSerializer.Deserialize<SensorValue>(content);



//                var _scope = _scopeFactory.CreateScope();
//                var _applicationContext = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

//                var sensors = _applicationContext.Sensors.ToList();

//                foreach (var sensor in sensors)
//                {
//                    if (sensor.Id == result.SensorId)
//                    {
//                        var sensorValue = new SensorValue { Value = result.Value, Created = DateTime.UtcNow };

//                        sensor.Values.Add(sensorValue);
//                    }

//                }

//                _applicationContext.SaveChangesAsync(stoppingToken);

//                Debug.WriteLine($"Получено сообщение: {content}");

//                _channel.BasicAck(ea.DeliveryTag, false);
//        };

//        _channel.BasicConsume("MyQueue", false, consumer);

//        return Task.CompletedTask;
//    }
//    public override void Dispose()
//    {
//        _channel.Close();
//        _connection.Close();
//        base.Dispose();
//    }
//}

public static class RandomExtensions
{
    public static double NextDouble(
        this Random random,
        double minValue,
        double maxValue)
    {
        return random.NextDouble() * (maxValue - minValue) + minValue;
    }
}

