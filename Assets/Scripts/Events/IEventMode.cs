using System;

namespace SRS.Events
{
    public interface IEventMode : IDisposable
    {
        event Action<EventResult> OnFinished;

        void Initialize(EventSession session);
        void Tick(float dt);
    }
}
