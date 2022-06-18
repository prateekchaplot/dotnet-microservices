using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _dataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
      IPlatformRepository repository,
      IMapper mapper,
      ICommandDataClient dataClient,
      IMessageBusClient messageBusClient)
    {
      _repository = repository;
      _mapper = mapper;
      _dataClient = dataClient;
      _messageBusClient = messageBusClient;
    }

    [HttpGet("[action]")]
    public IActionResult GetPlatforms()
    {
      var platforms = _repository.GetAllPlatforms();
      var dtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
      return Ok(dtos);
    }

    [HttpGet("[action]/{id}", Name = "GetPlatformById")]
    public IActionResult GetPlatformById(int id)
    {
      var platform = _repository.GetPlatformById(id);
      if (platform == null)
      {
        return NotFound();
      }
      
      var dto = _mapper.Map<PlatformReadDto>(platform);
      return Ok(dto);
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto platformCreate)
    {
      var platform = _mapper.Map<Platform>(platformCreate);
      _repository.CreatePlatform(platform);
      _repository.SaveChanges();

      var platformRead = _mapper.Map<PlatformReadDto>(platform);

      // Send Sync message
      try
      {
        await _dataClient.SendPlatformToCommand(platformRead);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Could not send synchronously: {ex.Message}");
      }

      // Send Async message
      try
      {
        var platformPublished = _mapper.Map<PlatformPublishedDto>(platformRead);
        platformPublished.Event = "Platform_Published";
        _messageBusClient.PublishNewPlatform(platformPublished);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Could not send asynchronously: {ex.Message}");
      }

      return CreatedAtRoute("GetPlatformById", new { id = platform.Id }, platformRead);
    }
  }
}