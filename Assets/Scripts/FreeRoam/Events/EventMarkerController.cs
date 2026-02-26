using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam.Events
{
    /// <summary>
    /// README:
    /// Handles world marker visuals and minimap icon metadata.
    /// MinimapController computes icon position each frame without allocations.
    /// </summary>
    public sealed class EventMarkerController : MonoBehaviour
    {
        [SerializeField] private string eventId;
        [SerializeField] private Sprite icon;
        [SerializeField] private Image worldIconImage;
        [SerializeField] private Transform worldAnchor;

        public string EventId => eventId;
        public Sprite Icon => icon;
        public Transform Anchor => worldAnchor != null ? worldAnchor : transform;

        public void Initialize(string id, Sprite markerIcon)
        {
            eventId = id;
            icon = markerIcon;

            if (worldIconImage != null)
            {
                worldIconImage.sprite = markerIcon;
            }
        }
    }
}
