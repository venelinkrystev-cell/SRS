using SRS.Events.Modes;
using SRS.Events.Routing;
using SRS.UI;
using UnityEngine;

namespace SRS.Events
{
    public sealed class EventLauncher : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private EventRegistry eventRegistry;
        [SerializeField] private Transform player;
        [SerializeField] private Checkpoint checkpointPrefab;
        [SerializeField] private Transform checkpointRoot;
        [SerializeField] private KeyCode cancelEventKey = KeyCode.Escape;

        [Header("UI")]
        [SerializeField] private EventPreviewPanel previewPanel;
        [SerializeField] private EventHUD eventHud;

        private EventSession _currentSession;
        private IEventMode _activeMode;

        public bool IsInEvent => _currentSession != null && _currentSession.State == EventSessionState.Running;

        private void Update()
        {
            if (_activeMode != null)
            {
                _activeMode.Tick(Time.deltaTime);
            }

            if (IsInEvent && Input.GetKeyDown(cancelEventKey))
            {
                CancelEvent();
            }
        }

        public void StartEvent(string eventId)
        {
            if (IsInEvent || _currentSession != null)
            {
                Debug.LogWarning("Event start ignored: another event session already exists.");
                return;
            }

            if (!eventRegistry.TryGetEvent(eventId, out var definition))
            {
                Debug.LogWarning($"Unknown event id '{eventId}'");
                return;
            }

            var freeRoamPosition = player != null ? player.position : Vector3.zero;
            var freeRoamRotation = player != null ? player.rotation : Quaternion.identity;

            _currentSession = new EventSession(definition, freeRoamPosition, freeRoamRotation);
            previewPanel.Show(definition, BeginEventFromPreview, CancelEvent);
        }

        public void CancelEvent()
        {
            if (_currentSession == null)
            {
                return;
            }

            _currentSession.MarkCancelled();
            CleanupAndReturnToFreeRoam(new EventResult
            {
                eventId = _currentSession.Definition.eventId,
                success = false,
                elapsedSeconds = Time.time - _currentSession.StartTime,
                completedLaps = 0,
                checkpointsPassed = 0
            });
        }

        public void FinishEvent(EventResult result)
        {
            if (_currentSession == null)
            {
                return;
            }

            _currentSession.MarkFinished();
            CleanupAndReturnToFreeRoam(result);
        }

        private void BeginEventFromPreview(string eventId)
        {
            if (_currentSession == null || _currentSession.Definition.eventId != eventId)
            {
                return;
            }

            previewPanel.Hide();
            _currentSession.MarkRunning();

            PlacePlayerAtRouteStart(_currentSession.Route);
            _activeMode = CreateMode(_currentSession.Definition.type);
            _activeMode.OnFinished += FinishEvent;
            _activeMode.Initialize(_currentSession);

            eventHud.Show(_currentSession.Definition.displayName);
            Debug.Log($"Event started: {_currentSession.Definition.eventId} ({_currentSession.Definition.type})");
        }

        private IEventMode CreateMode(EventType type)
        {
            switch (type)
            {
                case EventType.Circuit:
                    return new CircuitMode(checkpointPrefab, checkpointRoot, eventHud);
                case EventType.Sprint:
                default:
                    return new SprintMode(player, checkpointPrefab, checkpointRoot, eventHud);
            }
        }

        private void PlacePlayerAtRouteStart(RouteData route)
        {
            if (player == null || route == null)
            {
                return;
            }

            player.position = route.startPosition;
            var forward = route.startForward.sqrMagnitude > 0.001f ? route.startForward.normalized : Vector3.forward;
            player.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        private void CleanupAndReturnToFreeRoam(EventResult result)
        {
            previewPanel.Hide();
            eventHud.Hide();

            if (_activeMode != null)
            {
                _activeMode.OnFinished -= FinishEvent;
                _activeMode.Dispose();
                _activeMode = null;
            }

            if (player != null)
            {
                player.position = _currentSession.LastFreeRoamPosition;
                player.rotation = _currentSession.LastFreeRoamRotation;
            }

            Debug.Log($"Event ended: {result.eventId}, success={result.success}, elapsed={result.elapsedSeconds:0.00}s.");
            Debug.Log("Reward handler stub: progression/economy integration point.");

            _currentSession = null;
        }
    }
}
