using System.Collections.Generic;
using SRS.FreeRoam.Events;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam.Minimap
{
    /// <summary>
    /// README:
    /// URP-friendly minimap controller using an orthographic camera and RenderTexture.
    /// Assign camera output to RawImage.texture. Markers are projected in local map space.
    /// </summary>
    public sealed class MinimapController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private RectTransform minimapRect;
        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private RectTransform markerIconContainer;
        [SerializeField] private Image markerIconTemplate;

        [Header("Behavior")]
        [SerializeField] private float heightOffset = 120f;
        [SerializeField] private bool rotatePlayerIcon = true;
        [SerializeField] private bool rotateMapWithPlayer;
        [SerializeField] private float visibleWorldRadius = 160f;

        private Transform _player;
        private readonly List<EventMarkerController> _markers = new List<EventMarkerController>(16);
        private readonly List<RectTransform> _markerIcons = new List<RectTransform>(16);

        public void Initialize(Transform player, IReadOnlyList<EventMarkerController> markers)
        {
            _player = player;

            _markers.Clear();
            _markerIcons.Clear();

            if (markerIconTemplate == null)
            {
                return;
            }

            markerIconTemplate.gameObject.SetActive(false);

            for (int i = 0; i < markers.Count; i++)
            {
                EventMarkerController marker = markers[i];
                _markers.Add(marker);

                Image icon = Instantiate(markerIconTemplate, markerIconContainer);
                icon.sprite = marker.Icon;
                icon.gameObject.SetActive(true);
                _markerIcons.Add(icon.rectTransform);
            }
        }

        private void LateUpdate()
        {
            if (_player == null || minimapCamera == null)
            {
                return;
            }

            Vector3 playerPos = _player.position;
            minimapCamera.transform.position = new Vector3(playerPos.x, playerPos.y + heightOffset, playerPos.z);

            float yaw = _player.eulerAngles.y;
            if (rotateMapWithPlayer)
            {
                minimapCamera.transform.rotation = Quaternion.Euler(90f, yaw, 0f);
                if (playerIcon != null) playerIcon.localEulerAngles = Vector3.zero;
            }
            else
            {
                minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                if (playerIcon != null && rotatePlayerIcon)
                {
                    playerIcon.localEulerAngles = new Vector3(0f, 0f, -yaw);
                }
            }

            UpdateMarkerIcons(playerPos, yaw);
        }

        private void UpdateMarkerIcons(Vector3 playerPos, float yaw)
        {
            if (minimapRect == null)
            {
                return;
            }

            float rectHalfWidth = minimapRect.rect.width * 0.5f;
            float rectHalfHeight = minimapRect.rect.height * 0.5f;
            float invRadius = visibleWorldRadius > 0.001f ? 1f / visibleWorldRadius : 0f;

            for (int i = 0; i < _markers.Count; i++)
            {
                EventMarkerController marker = _markers[i];
                RectTransform icon = _markerIcons[i];
                Vector3 delta = marker.Anchor.position - playerPos;

                if (rotateMapWithPlayer)
                {
                    delta = Quaternion.Euler(0f, -yaw, 0f) * delta;
                }

                float nx = Mathf.Clamp(delta.x * invRadius, -1f, 1f);
                float ny = Mathf.Clamp(delta.z * invRadius, -1f, 1f);

                icon.anchoredPosition = new Vector2(nx * rectHalfWidth, ny * rectHalfHeight);
            }
        }
    }
}
