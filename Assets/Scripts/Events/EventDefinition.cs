using UnityEngine;

namespace SRS.Events
{
    public enum EventType
    {
        Sprint = 0,
        Circuit = 1
    }

    [CreateAssetMenu(menuName = "SRS/Events/Event Definition", fileName = "EventDefinition")]
    public sealed class EventDefinition : ScriptableObject
    {
        public string eventId;
        public string eventName;
        public EventType eventType;
        public long baseCashReward = 100;
        public int baseRespectReward = 10;
    }
}
