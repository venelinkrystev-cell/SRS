using System.Collections.Generic;
using UnityEngine;

namespace SRS.Events
{
    [CreateAssetMenu(fileName = "EventRegistry", menuName = "SRS/Events/Event Registry")]
    public sealed class EventRegistry : ScriptableObject
    {
        [SerializeField] private List<EventDefinition> events = new List<EventDefinition>();
        private readonly Dictionary<string, EventDefinition> _lookup = new Dictionary<string, EventDefinition>(64);

        public IReadOnlyList<EventDefinition> Events => events;

        public bool TryGetEvent(string eventId, out EventDefinition definition)
        {
            if (_lookup.Count != events.Count)
            {
                RebuildLookup();
            }

            return _lookup.TryGetValue(eventId, out definition);
        }

        private void OnValidate()
        {
            RebuildLookup();
        }

        private void OnEnable()
        {
            RebuildLookup();
        }

        private void RebuildLookup()
        {
            _lookup.Clear();
            for (var i = 0; i < events.Count; i++)
            {
                var definition = events[i];
                if (definition == null || !definition.IsValid() || _lookup.ContainsKey(definition.eventId))
                {
                    continue;
                }

                _lookup.Add(definition.eventId, definition);
            }
        }
    }
}
