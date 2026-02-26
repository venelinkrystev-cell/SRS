using UnityEngine;
using SRS.Events.Routing;

namespace SRS.Events
{
    public enum EventSessionState
    {
        Preview = 0,
        Running = 1,
        Finished = 2,
        Cancelled = 3
    }

    public struct EventResult
    {
        public string eventId;
        public bool success;
        public float elapsedSeconds;
        public int completedLaps;
        public int checkpointsPassed;
    }

    public sealed class EventSession
    {
        public EventDefinition Definition { get; private set; }
        public RouteData Route => Definition.route;
        public float StartTime { get; private set; }
        public EventSessionState State { get; private set; }
        public Vector3 LastFreeRoamPosition { get; private set; }
        public Quaternion LastFreeRoamRotation { get; private set; }

        public EventSession(EventDefinition definition, Vector3 lastFreeRoamPosition, Quaternion lastFreeRoamRotation)
        {
            Definition = definition;
            LastFreeRoamPosition = lastFreeRoamPosition;
            LastFreeRoamRotation = lastFreeRoamRotation;
            StartTime = Time.time;
            State = EventSessionState.Preview;
        }

        public void MarkRunning()
        {
            State = EventSessionState.Running;
            StartTime = Time.time;
        }

        public void MarkFinished()
        {
            State = EventSessionState.Finished;
        }

        public void MarkCancelled()
        {
            State = EventSessionState.Cancelled;
        }
    }
}
