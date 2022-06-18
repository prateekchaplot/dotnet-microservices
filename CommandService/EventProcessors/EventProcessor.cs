using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Enums;
using CommandService.Models;

namespace CommandService.EventProcessors
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);
      switch (eventType)
      {
        case EventType.PlatformPublished:
          AddPlatform(message);
          break;
      }
    }

    private static EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event");
      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Platform_Published":
          Console.WriteLine("--> Platform Published Event detected!");
          return EventType.PlatformPublished;
      }

      Console.WriteLine("--> Could not detect the event type!");
      return EventType.Undetermined;
    }

    private void AddPlatform(string platformPublishedMessage)
    {
      using var scope = _scopeFactory.CreateScope();
      var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
      var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

      try
      {
        var platformModel = _mapper.Map<Platform>(platformPublishedDto);
        var externalPlatformExists = repository.ExternalPlatformExists(platformModel.ExternalId);
        if (!externalPlatformExists)
        {
          repository.CreatePlatform(platformModel);
          repository.SaveChanges();
        }
        else
        {
          Console.WriteLine("--> Platform already exists!");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not add platform to Database: {ex.Message}");
      }
    }
  }
}