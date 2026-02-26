using UnityEngine;
using SRS.Events.Routing;

namespace SRS.Events
{
    public enum EventType
    {
        Sprint = 0,
        Circuit = 1
    }

    [CreateAssetMenu(fileName = "EventDefinition", menuName = "SRS/Events/Event Definition")]
    public sealed class EventDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string eventId;
        public string displayName;
        public EventType type;
        public Sprite icon;

        [Header("Mode Data")]
        public RouteData route;
        [Min(1)] public int laps = 3;
        [Min(0f)] public float timeLimitSeconds;

        [Header("Progression Placeholders")]
        [Min(0)] public int recommendedRespect;
        [Min(0)] public int baseCashReward;
        [Min(0)] public int baseRespectReward;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(eventId) && route != null;
        }
    }
}
