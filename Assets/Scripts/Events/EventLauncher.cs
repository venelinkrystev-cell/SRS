using SRS.Progression;
using SRS.UI;
using UnityEngine;

namespace SRS.Events
{
    public sealed class EventLauncher : MonoBehaviour
    {
        [SerializeField] private EventDefinition currentEvent;
        [SerializeField] private EventSession currentSession;
        [SerializeField] private ResultsScreen resultsScreen;

        private void OnEnable()
        {
            if (currentSession == null)
            {
                return;
            }

            currentSession.EventFinished += FinishEvent;
            currentSession.EventCancelled += CancelEvent;
        }

        private void OnDisable()
        {
            if (currentSession == null)
            {
                return;
            }

            currentSession.EventFinished -= FinishEvent;
            currentSession.EventCancelled -= CancelEvent;
        }

        public void FinishEvent(EventResult result)
        {
            if (currentEvent == null || ProgressionService.Instance == null)
            {
                Debug.LogWarning("[EventLauncher] Missing event definition or progression service.");
                ReturnToFreeRoam();
                return;
            }

            var reward = ProgressionService.Instance.ApplyEventResult(currentEvent.eventId, currentEvent, result);

            if (!result.wasCancelled && resultsScreen != null)
            {
                resultsScreen.Open(currentEvent, result, reward, ReturnToFreeRoam);
                return;
            }

            ReturnToFreeRoam();
        }

        public void CancelEvent(EventResult result)
        {
            ReturnToFreeRoam();
        }

        private void ReturnToFreeRoam()
        {
            // Existing 1.2 return flow should be called from here.
            Debug.Log("[EventLauncher] Returning to Free Roam.");
        }
    }
}
