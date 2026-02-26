using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam.Events
{
    /// <summary>
    /// README:
    /// Stub event launcher used by EventTriggerZone.
    /// Replace StartEvent internals with race loading logic in future tasks.
    /// </summary>
    public sealed class EventLauncher : MonoBehaviour, IEventLauncher
    {
        [Header("Optional UI")]
        [SerializeField] private CanvasGroup loadingGroup;
        [SerializeField] private Text loadingText;
        [SerializeField] private float loadingDisplaySeconds = 1.2f;

        private Coroutine _loadingRoutine;

        public void StartEvent(string eventId)
        {
            Debug.Log($"[EventLauncher] StartEvent called for '{eventId}'.");

            if (loadingGroup == null)
            {
                return;
            }

            if (_loadingRoutine != null)
            {
                StopCoroutine(_loadingRoutine);
            }

            _loadingRoutine = StartCoroutine(ShowLoadingRoutine(eventId));
        }

        private IEnumerator ShowLoadingRoutine(string eventId)
        {
            if (loadingText != null)
            {
                loadingText.text = $"Loading event {eventId}...";
            }

            loadingGroup.alpha = 1f;
            loadingGroup.gameObject.SetActive(true);

            yield return new WaitForSeconds(loadingDisplaySeconds);

            loadingGroup.alpha = 0f;
            loadingGroup.gameObject.SetActive(false);
            _loadingRoutine = null;
        }
    }
}
