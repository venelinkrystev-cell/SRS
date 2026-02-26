using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRS.FreeRoam.Events
{
    /// <summary>
    /// README:
    /// Data-driven registry for free-roam events.
    /// Create one EventRegistry asset and assign it in FreeRoamBootstrap.
    /// Supports either fixed worldPosition or optional transformOverride.
    /// </summary>
    [CreateAssetMenu(menuName = "SRS/Free Roam/Event Registry", fileName = "EventRegistry")]
    public sealed class EventRegistry : ScriptableObject
    {
        [Serializable]
        public sealed class EventDefinition
        {
            public string eventId;
            public string displayName;
            public Vector3 worldPosition;
            public Transform transformOverride;
            public Sprite icon;
            [Min(1f)] public float triggerRadius = 10f;

            public Vector3 ResolvePosition()
            {
                return transformOverride != null ? transformOverride.position : worldPosition;
            }
        }

        [SerializeField] private List<EventDefinition> events = new List<EventDefinition>(8);

        public IReadOnlyList<EventDefinition> Events => events;

        public bool TryGetEvent(string eventId, out EventDefinition definition)
        {
            for (int i = 0; i < events.Count; i++)
            {
                EventDefinition e = events[i];
                if (e != null && string.Equals(e.eventId, eventId, StringComparison.Ordinal))
                {
                    definition = e;
                    return true;
                }
            }

            definition = null;
            return false;
        }
    }
}
