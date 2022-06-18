using CommandService.Models;
using CommandService.SyncDataServices;

namespace CommandService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
      using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
      var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
      var platforms = grpcClient!.ReturnAllPlatforms();

      var repository = serviceScope.ServiceProvider.GetService<ICommandRepository>();
      SeedData(repository!, platforms);
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
      Console.WriteLine("--> Seeding New Platforms...");

      foreach (var platform in platforms)
      {
        if (!repository.ExternalPlatformExists(platform.ExternalId))
        {
          repository.CreatePlatform(platform);
        }
      }

      repository.SaveChanges();
    }
  }
}