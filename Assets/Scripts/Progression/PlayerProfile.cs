using System;
using System.Collections.Generic;

namespace SRS.Progression
{
    [Serializable]
    public sealed class PlayerProfile
    {
        public const int CurrentSchemaVersion = 1;

        public int schemaVersion = CurrentSchemaVersion;
        public long cash;
        public int respect;
        public HashSet<string> completedEventIds = new HashSet<string>();
        public int totalEventsCompleted;
        public int totalWins;
        public double totalPlayTimeSeconds;
        public string lastActiveCarId = string.Empty;

        public static PlayerProfile CreateDefault()
        {
            return new PlayerProfile
            {
                schemaVersion = CurrentSchemaVersion,
                cash = 0,
                respect = 0,
                completedEventIds = new HashSet<string>(),
                totalEventsCompleted = 0,
                totalWins = 0,
                totalPlayTimeSeconds = 0d,
                lastActiveCarId = string.Empty
            };
        }

        public bool MarkEventCompleted(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return false;
            }

            return completedEventIds.Add(eventId);
        }

        public bool HasCompletedEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return false;
            }

            return completedEventIds.Contains(eventId);
        }

        public PlayerProfileSaveData ToSaveData()
        {
            return new PlayerProfileSaveData
            {
                schemaVersion = schemaVersion,
                cash = cash,
                respect = respect,
                completedEventIds = new List<string>(completedEventIds),
                totalEventsCompleted = totalEventsCompleted,
                totalWins = totalWins,
                totalPlayTimeSeconds = totalPlayTimeSeconds,
                lastActiveCarId = lastActiveCarId
            };
        }

        public static PlayerProfile FromSaveData(PlayerProfileSaveData data)
        {
            var profile = CreateDefault();

            profile.schemaVersion = data.schemaVersion;
            profile.cash = data.cash;
            profile.respect = data.respect;
            profile.totalEventsCompleted = data.totalEventsCompleted;
            profile.totalWins = data.totalWins;
            profile.totalPlayTimeSeconds = data.totalPlayTimeSeconds;
            profile.lastActiveCarId = data.lastActiveCarId ?? string.Empty;

            profile.completedEventIds = data.completedEventIds != null
                ? new HashSet<string>(data.completedEventIds)
                : new HashSet<string>();

            return profile;
        }
    }

    [Serializable]
    public sealed class PlayerProfileSaveData
    {
        public int schemaVersion = PlayerProfile.CurrentSchemaVersion;
        public long cash;
        public int respect;
        public List<string> completedEventIds = new List<string>();
        public int totalEventsCompleted;
        public int totalWins;
        public double totalPlayTimeSeconds;
        public string lastActiveCarId = string.Empty;
    }
}
