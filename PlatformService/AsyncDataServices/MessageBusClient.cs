using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient, IDisposable
  {
    private readonly IConnection _connection;
    private readonly IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
      var factory = new ConnectionFactory()
      {
        HostName = configuration["RabbitMQHost"],
        Port = int.Parse(configuration["RabbitMQPort"])
      };

      try
      {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Connected to Message Bus");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
      }
    }
    
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);
      if (_connection!.IsOpen)
      {
        Console.WriteLine("--> RabbitMQ connection is open, sending message...");
        SendMessage(message);
      }
      else
      {
        Console.WriteLine("--> RabbitMQ connection is closed!");
      }
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);
      
      _channel.BasicPublish(
        exchange: "trigger",
        routingKey: "",
        basicProperties: null,
        body: body);

      Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
      Console.WriteLine("--> Message Bus disposed!");
      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }

      GC.SuppressFinalize(this);
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs eventArgs)
    {
      Console.WriteLine("--> RabbitMQ Connection Shutdown!");
    }
  }
}