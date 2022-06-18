using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
  [Route("api/c/platforms/{platformId}/[controller]")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommandReadDto))]
    public IActionResult GetCommandsForPlatform(int platformId)
    {
      var platformExists = _repository.PlatformExists(platformId);
      if (!platformExists)
      {
        return NotFound();
      }

      var commands = _repository.GetCommandsForPlatform(platformId);
      var dtos = _mapper.Map<IEnumerable<CommandReadDto>>(commands);
      return Ok(dtos);
    }

    [HttpGet("{commandId}", Name = "GetCommand")]
    public IActionResult GetCommand(int platformId, int commandId)
    {
      var platformExists = _repository.PlatformExists(platformId);
      if (!platformExists)
      {
        return NotFound();
      }

      var command = _repository.GetCommand(platformId, commandId);
      if (command == null)
      {
        return NotFound();
      }

      var dto = _mapper.Map<CommandReadDto>(command);
      return Ok(dto);
    }

    [HttpPost]
    public IActionResult Create(int platformId, [FromBody] CommandCreateDto commandCreate)
    {
      var platformExists = _repository.PlatformExists(platformId);
      if (!platformExists)
      {
        return NotFound();
      }

      var command = _mapper.Map<Command>(commandCreate);
      _repository.Create(platformId, command);
      _repository.SaveChanges();

      var dto = _mapper.Map<CommandReadDto>(command);
      return CreatedAtRoute("GetCommand", new { platformId, commandId = command.Id }, dto);
    }
  }
}