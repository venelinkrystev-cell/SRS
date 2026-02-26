using System;
using SRS.Progression;
using UnityEngine;

namespace SRS.Events
{
    public sealed class EventSession : MonoBehaviour
    {
        public event Action<EventResult> EventFinished;
        public event Action<EventResult> EventCancelled;

        private EventResult _result;

        public void StartSession()
        {
            _result = new EventResult
            {
                finished = false,
                placement = 1,
                finishTimeSeconds = 0f,
                opponentsCount = 0,
                checkpointsPassed = 0,
                lapsCompleted = 0,
                wasCancelled = false,
                wasDisqualified = false
            };
        }

        public void Complete(int placement, float finishTimeSeconds, int checkpointsPassed, int lapsCompleted, int opponentsCount, bool wasDisqualified)
        {
            _result.finished = true;
            _result.wasCancelled = false;
            _result.placement = placement <= 0 ? 1 : placement;
            _result.finishTimeSeconds = finishTimeSeconds;
            _result.checkpointsPassed = checkpointsPassed;
            _result.lapsCompleted = lapsCompleted;
            _result.opponentsCount = opponentsCount;
            _result.wasDisqualified = wasDisqualified;

            EventFinished?.Invoke(_result);
        }

        public void Cancel()
        {
            _result = EventResult.Cancelled();
            EventCancelled?.Invoke(_result);
        }
    }
}
