using UnityEngine;

namespace SRS.Events
{
    [RequireComponent(typeof(Collider))]
    public sealed class EventTriggerZone : MonoBehaviour
    {
        [SerializeField] private string eventId;
        [SerializeField] private EventLauncher eventLauncher;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private bool _playerInside;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void Update()
        {
            if (!_playerInside || eventLauncher == null || eventLauncher.IsInEvent)
            {
                return;
            }

            if (Input.GetKeyDown(interactKey))
            {
                eventLauncher.StartEvent(eventId);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _playerInside = true;
            Debug.Log($"Press {interactKey} to view event");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _playerInside = false;
        }
    }
}
