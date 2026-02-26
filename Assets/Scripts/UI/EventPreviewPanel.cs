using System;
using SRS.Events;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.UI
{
    public sealed class EventPreviewPanel : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text typeText;
        [SerializeField] private Text lapsText;
        [SerializeField] private Text rewardsText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button cancelButton;

        private string _eventId;
        private Action<string> _onStart;
        private Action _onCancel;

        private void Awake()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }

            gameObject.SetActive(false);
        }

        public void Show(EventDefinition definition, Action<string> onStart, Action onCancel)
        {
            _eventId = definition.eventId;
            _onStart = onStart;
            _onCancel = onCancel;

            if (titleText != null)
            {
                titleText.text = definition.displayName;
            }

            if (typeText != null)
            {
                typeText.text = definition.type.ToString();
            }

            if (lapsText != null)
            {
                lapsText.text = definition.type == EventType.Circuit ? $"Laps: {definition.laps}" : "Laps: -";
            }

            if (rewardsText != null)
            {
                rewardsText.text = $"Cash: {definition.baseCashReward} | Respect: {definition.baseRespectReward}";
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _eventId = string.Empty;
            _onStart = null;
            _onCancel = null;
        }

        private void OnStartClicked()
        {
            _onStart?.Invoke(_eventId);
        }

        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
        }
    }
}
