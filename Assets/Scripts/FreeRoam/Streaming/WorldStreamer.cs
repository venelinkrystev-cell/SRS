using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam.Streaming
{
    /// <summary>
    /// README:
    /// Sector-based world streamer for open-world free roam.
    /// Organize city content under sector roots and register them in the inspector.
    /// Keeps current + neighbor sectors active with optional debug overlay.
    /// </summary>
    public sealed class WorldStreamer : MonoBehaviour
    {
        [Serializable]
        public sealed class Sector
        {
            public Vector2Int sectorId;
            public GameObject root;
        }

        [Header("Setup")]
        [SerializeField] private Transform player;
        [SerializeField] private float sectorSize = 200f;
        [SerializeField] private int activeRadius = 1;
        [SerializeField] private int hysteresisMargin = 1;
        [SerializeField] private List<Sector> sectors = new List<Sector>(64);

        [Header("Debug Overlay")]
        [SerializeField] private bool showDebugOverlay = true;
        [SerializeField] private Text debugText;

        private readonly Dictionary<Vector2Int, Sector> _sectorsById = new Dictionary<Vector2Int, Sector>(64);
        private readonly HashSet<Vector2Int> _activeSet = new HashSet<Vector2Int>();
        private readonly List<Vector2Int> _toDeactivate = new List<Vector2Int>(64);
        private Vector2Int _currentSector;
        private float _fps;

        public Vector2Int CurrentSector => _currentSector;
        public int ActiveSectorCount => _activeSet.Count;

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        private void Awake()
        {
            BuildLookup();
            _currentSector = new Vector2Int(int.MinValue, int.MinValue);
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            _fps = Mathf.Lerp(_fps, 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f), 0.1f);

            Vector2Int sector = WorldToSector(player.position);
            if (sector != _currentSector)
            {
                _currentSector = sector;
                RefreshActiveSectors();
            }

            if (showDebugOverlay && debugText != null)
            {
                debugText.text = $"Sector: {_currentSector.x},{_currentSector.y}\nActive: {_activeSet.Count}\nFPS: {(int)_fps}";
            }
        }

        private void BuildLookup()
        {
            _sectorsById.Clear();
            for (int i = 0; i < sectors.Count; i++)
            {
                Sector sector = sectors[i];
                if (sector == null || sector.root == null)
                {
                    continue;
                }

                _sectorsById[sector.sectorId] = sector;
                sector.root.SetActive(false);
            }
        }

        private void RefreshActiveSectors()
        {
            int minX = _currentSector.x - activeRadius;
            int maxX = _currentSector.x + activeRadius;
            int minY = _currentSector.y - activeRadius;
            int maxY = _currentSector.y + activeRadius;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2Int id = new Vector2Int(x, y);
                    if (_activeSet.Contains(id))
                    {
                        continue;
                    }

                    if (_sectorsById.TryGetValue(id, out Sector sector))
                    {
                        _activeSet.Add(id);
                        sector.root.SetActive(true);
                    }
                }
            }

            _toDeactivate.Clear();
            foreach (Vector2Int id in _activeSet)
            {
                if (Mathf.Abs(id.x - _currentSector.x) <= activeRadius + hysteresisMargin &&
                    Mathf.Abs(id.y - _currentSector.y) <= activeRadius + hysteresisMargin)
                {
                    continue;
                }

                _toDeactivate.Add(id);
            }

            for (int i = 0; i < _toDeactivate.Count; i++)
            {
                Vector2Int id = _toDeactivate[i];
                _activeSet.Remove(id);

                if (_sectorsById.TryGetValue(id, out Sector sector))
                {
                    sector.root.SetActive(false);
                }
            }
        }

        private Vector2Int WorldToSector(Vector3 position)
        {
            float inv = sectorSize > 0.001f ? 1f / sectorSize : 0.005f;
            return new Vector2Int(Mathf.FloorToInt(position.x * inv), Mathf.FloorToInt(position.z * inv));
        }
    }
}
