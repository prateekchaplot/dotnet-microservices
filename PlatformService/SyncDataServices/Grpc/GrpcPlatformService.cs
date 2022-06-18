using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
  public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
  {
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;

    public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequests request, ServerCallContext context)
    {
      var response = new PlatformResponse();
      var platforms = _repository.GetAllPlatforms();

      var grpcPlatforms = _mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms);
      response.Platforms.AddRange(grpcPlatforms);
      
      return Task.FromResult(response);
    }
  }
}