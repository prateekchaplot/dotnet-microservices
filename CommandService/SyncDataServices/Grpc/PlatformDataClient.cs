using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;

namespace CommandService.SyncDataServices
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
      _configuration = configuration;
      _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      Console.WriteLine($"--> Calling gRPC service: {_configuration["GrpcPlatform"]}");
      var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
      var client = new GrpcPlatform.GrpcPlatformClient(channel);
      var request = new GetAllRequests();

      IEnumerable<Platform> result = new List<Platform>();
      
      try
      {
        var reply = client.GetAllPlatforms(request);
        result = _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not call gRPC server: {ex.Message}");
      }

      return result;
    }
  }
}