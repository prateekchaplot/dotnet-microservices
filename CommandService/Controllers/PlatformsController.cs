using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
  [Route("api/c/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
      var platforms = _repository.GetAllPlatforms();
      var dtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
      return Ok(dtos);
    }

    [HttpPost("[action]")]
    public IActionResult TestInboundConnection()
    {
      Console.WriteLine("--> Test inbound connection");
      return Ok();
    }
  }
}