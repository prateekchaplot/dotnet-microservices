namespace CommandService.EventProcessors
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}