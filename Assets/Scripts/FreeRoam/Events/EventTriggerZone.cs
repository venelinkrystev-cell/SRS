using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam.Events
{
    /// <summary>
    /// README:
    /// Attach to a trigger collider to expose event interaction in free roam.
    /// Shows a prompt while player is inside and listens for E key.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public sealed class EventTriggerZone : MonoBehaviour
    {
        [SerializeField] private string eventId;
        [SerializeField] private Text promptText;
        [SerializeField] private string promptMessage = "Press E to Start Event";
        [SerializeField] private string playerTag = "Player";

        private IEventLauncher _eventLauncher;
        private bool _playerInside;

        public void Initialize(string id, float radius, IEventLauncher launcher, Text prompt)
        {
            eventId = id;
            _eventLauncher = launcher;
            promptText = prompt;

            SphereCollider collider = GetComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = Mathf.Max(1f, radius);
        }

        private void Awake()
        {
            SphereCollider collider = GetComponent<SphereCollider>();
            collider.isTrigger = true;
            HidePrompt();
        }

        private void Update()
        {
            if (_playerInside && Input.GetKeyDown(KeyCode.E))
            {
                _eventLauncher?.StartEvent(eventId);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag))
            {
                return;
            }

            _playerInside = true;
            ShowPrompt();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(playerTag))
            {
                return;
            }

            _playerInside = false;
            HidePrompt();
        }

        private void ShowPrompt()
        {
            if (promptText == null)
            {
                return;
            }

            promptText.text = promptMessage;
            promptText.gameObject.SetActive(true);
        }

        private void HidePrompt()
        {
            if (promptText == null)
            {
                return;
            }

            promptText.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            SphereCollider sphere = GetComponent<SphereCollider>();
            float radius = sphere != null ? sphere.radius : 1f;
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.35f);
            Gizmos.DrawSphere(transform.position, radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}
