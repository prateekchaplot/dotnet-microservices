using CommandService.Models;

namespace CommandService.Data
{
  public class CommandRepository : ICommandRepository
  {
    private readonly AppDbContext _context;

    public CommandRepository(AppDbContext context)
    {
      _context = context;
    }

    public void Create(int platformId, Command command)
    {
      if (command == null)
      {
        throw new ArgumentNullException(nameof(Command));
      }

      command.PlatformId = platformId;
      _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
      if (platform == null)
      {
        throw new ArgumentNullException(nameof(Platform));
      }

      _context.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
      return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      return _context.Platforms;
    }

    public Command GetCommand(int platformId, int commandId)
    {
      return _context.Commands
        .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
      return _context.Commands
        .Where(c => c.PlatformId == platformId)
        .OrderBy(c => c.Platform.Name);
    }

    public bool PlatformExists(int platformId)
    {
      return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() > 0;
    }
  }
}