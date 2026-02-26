namespace SRS.FreeRoam.Events
{
    /// <summary>
    /// Abstraction for launching race events from free roam.
    /// </summary>
    public interface IEventLauncher
    {
        void StartEvent(string eventId);
    }
}
