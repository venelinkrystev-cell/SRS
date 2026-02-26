using System;
using UnityEngine;

namespace SRS.Progression
{
    [Serializable]
    public struct EventResult
    {
        public bool finished;
        public int placement;
        public float finishTimeSeconds;
        public int opponentsCount;
        public int checkpointsPassed;
        public int lapsCompleted;
        public bool wasCancelled;
        public bool wasDisqualified;

        public static EventResult Cancelled()
        {
            return new EventResult
            {
                finished = false,
                placement = 0,
                finishTimeSeconds = 0f,
                opponentsCount = 0,
                checkpointsPassed = 0,
                lapsCompleted = 0,
                wasCancelled = true,
                wasDisqualified = false
            };
        }
    }

    [Serializable]
    public struct RewardBreakdown
    {
        public long baseCash;
        public long placementBonusCash;
        public long cleanBonusCash;
        public long penaltiesCash;
        public long totalCash;

        public int baseRespect;
        public int placementBonusRespect;
        public int penaltiesRespect;
        public int totalRespect;

        public static RewardBreakdown Zero => new RewardBreakdown();
    }

    [Serializable]
    public struct RewardContext
    {
        public long baseCashReward;
        public int baseRespectReward;
        public bool cleanFinish;
    }

    [Serializable]
    public enum EventOutcomeState
    {
        Unknown = 0,
        Completed = 1,
        Cancelled = 2,
        Disqualified = 3
    }
}
