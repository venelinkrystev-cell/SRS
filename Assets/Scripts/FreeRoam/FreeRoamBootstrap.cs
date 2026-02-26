using System.Collections.Generic;
using SRS.FreeRoam.Events;
using SRS.FreeRoam.Minimap;
using SRS.FreeRoam.Streaming;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.FreeRoam
{
    /// <summary>
    /// README:
    /// Free-roam composition root.
    /// - Finds PlayerSpawn
    /// - Spawns player car
    /// - Builds event markers + trigger zones from EventRegistry
    /// - Initializes minimap and world streamer
    /// </summary>
    public sealed class FreeRoamBootstrap : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameObject playerCarPrefab;
        [SerializeField] private EventRegistry eventRegistry;
        [SerializeField] private EventLauncher eventLauncher;

        [Header("Scene References")]
        [SerializeField] private MinimapController minimapController;
        [SerializeField] private WorldStreamer worldStreamer;
        [SerializeField] private EventMarkerController eventMarkerPrefab;
        [SerializeField] private EventTriggerZone eventTriggerZonePrefab;
        [SerializeField] private Transform markersRoot;
        [SerializeField] private Transform triggersRoot;
        [SerializeField] private Text interactionPromptText;

        [Header("Fallback")]
        [SerializeField] private string spawnTag = "PlayerSpawn";

        private readonly List<EventMarkerController> _spawnedMarkers = new List<EventMarkerController>(16);

        private void Start()
        {
            Transform spawn = FindSpawnPoint();
            Transform player = SpawnPlayer(spawn);

            if (player == null)
            {
                Debug.LogError("[FreeRoamBootstrap] Could not initialize free roam because player is null.");
                return;
            }

            SpawnEvents();

            if (minimapController != null)
            {
                minimapController.Initialize(player, _spawnedMarkers);
            }

            if (worldStreamer != null)
            {
                worldStreamer.SetPlayer(player);
            }
        }

        private Transform FindSpawnPoint()
        {
            GameObject spawn = GameObject.FindGameObjectWithTag(spawnTag);
            if (spawn != null)
            {
                return spawn.transform;
            }

            Debug.LogWarning($"[FreeRoamBootstrap] Spawn with tag '{spawnTag}' not found. Using origin.");
            return null;
        }

        private Transform SpawnPlayer(Transform spawn)
        {
            if (playerCarPrefab == null)
            {
                Debug.LogError("[FreeRoamBootstrap] playerCarPrefab is not assigned.");
                return null;
            }

            Vector3 pos = spawn != null ? spawn.position : Vector3.zero;
            Quaternion rot = spawn != null ? spawn.rotation : Quaternion.identity;

            GameObject instance = Instantiate(playerCarPrefab, pos, rot);
            instance.name = "PlayerCar";
            instance.tag = "Player";
            return instance.transform;
        }

        private void SpawnEvents()
        {
            _spawnedMarkers.Clear();

            if (eventRegistry == null)
            {
                Debug.LogWarning("[FreeRoamBootstrap] No event registry assigned.");
                return;
            }

            for (int i = 0; i < eventRegistry.Events.Count; i++)
            {
                EventRegistry.EventDefinition def = eventRegistry.Events[i];
                if (def == null)
                {
                    continue;
                }

                Vector3 pos = def.ResolvePosition();

                if (eventMarkerPrefab != null)
                {
                    EventMarkerController marker = Instantiate(eventMarkerPrefab, pos, Quaternion.identity, markersRoot);
                    marker.name = $"EventMarker_{def.eventId}";
                    marker.Initialize(def.eventId, def.icon);
                    _spawnedMarkers.Add(marker);
                }

                if (eventTriggerZonePrefab != null)
                {
                    EventTriggerZone trigger = Instantiate(eventTriggerZonePrefab, pos, Quaternion.identity, triggersRoot);
                    trigger.name = $"EventTrigger_{def.eventId}";
                    trigger.Initialize(def.eventId, def.triggerRadius, eventLauncher, interactionPromptText);
                }
            }
        }
    }
}
