using System;
using SRS.Events;
using UnityEngine;

namespace SRS.Progression
{
    public sealed class ProgressionService : MonoBehaviour
    {
        public static ProgressionService Instance { get; private set; }

        [SerializeField] private bool dontDestroyOnLoad = true;

        private PlayerProfileStore _profileStore;
        private PlayerProfile _profile;

        public event Action<PlayerProfile> ProfileChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            _profileStore = new PlayerProfileStore();
            _profile = _profileStore.LoadOrCreate();
        }

        public PlayerProfile GetProfile()
        {
            return _profile;
        }

        public RewardBreakdown ApplyEventResult(string eventId, EventDefinition definition, EventResult result)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (result.wasCancelled)
            {
                return RewardBreakdown.Zero;
            }

            var context = new RewardContext
            {
                baseCashReward = definition.baseCashReward,
                baseRespectReward = definition.baseRespectReward,
                cleanFinish = true
            };

            var breakdown = RewardCalculator.Calculate(context, result);

            AddCash(breakdown.totalCash);
            AddRespect(breakdown.totalRespect);

            if (result.finished && !result.wasDisqualified)
            {
                if (_profile.MarkEventCompleted(eventId))
                {
                    _profile.totalEventsCompleted += 1;
                }

                if (result.placement <= 1)
                {
                    _profile.totalWins += 1;
                }
            }

            Save();

            Debug.Log($"[Progression] RewardApplied event={eventId} placement={result.placement} totalCash={breakdown.totalCash} totalRespect={breakdown.totalRespect}");
            ProfileChanged?.Invoke(_profile);

            return breakdown;
        }

        public void AddCash(long amount)
        {
            _profile.cash += amount;
            ProfileChanged?.Invoke(_profile);
        }

        public void AddRespect(int amount)
        {
            _profile.respect += amount;
            ProfileChanged?.Invoke(_profile);
        }

        public void Save()
        {
            _profileStore.Save(_profile);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Debug.Log($"[Progression] Profile cash={_profile.cash} respect={_profile.respect} completed={_profile.totalEventsCompleted} wins={_profile.totalWins}");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                AddCash(1000);
                AddRespect(10);
                Save();
                Debug.Log("[Progression] Debug grant: +1000 cash, +10 respect");
            }
        }
#endif
    }
}
