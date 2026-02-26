using System;
using SRS.Events;
using SRS.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace SRS.UI
{
    public sealed class ResultsScreen : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Text eventNameText;
        [SerializeField] private Text eventTypeText;
        [SerializeField] private Text placementText;
        [SerializeField] private Text finishTimeText;

        [Header("Rows")]
        [SerializeField] private ResultsBreakdownRow baseCashRow;
        [SerializeField] private ResultsBreakdownRow placementCashRow;
        [SerializeField] private ResultsBreakdownRow cleanCashRow;
        [SerializeField] private ResultsBreakdownRow penaltiesCashRow;
        [SerializeField] private ResultsBreakdownRow baseRespectRow;
        [SerializeField] private ResultsBreakdownRow placementRespectRow;
        [SerializeField] private ResultsBreakdownRow penaltiesRespectRow;

        [Header("Totals")]
        [SerializeField] private Text totalCashText;
        [SerializeField] private Text totalRespectText;

        [Header("Actions")]
        [SerializeField] private Button continueButton;

        private Action _onContinue;

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(HandleContinueClicked);
            }

            gameObject.SetActive(false);
        }

        public void Open(EventDefinition definition, EventResult result, RewardBreakdown breakdown, Action onContinue)
        {
            _onContinue = onContinue;

            if (eventNameText != null)
            {
                eventNameText.text = definition != null ? definition.eventName : "Unknown Event";
            }

            if (eventTypeText != null)
            {
                eventTypeText.text = definition != null ? definition.eventType.ToString() : "Unknown";
            }

            if (placementText != null)
            {
                placementText.text = result.wasDisqualified ? "Disqualified" : $"Placement: {Mathf.Max(1, result.placement)}";
            }

            if (finishTimeText != null)
            {
                finishTimeText.text = result.finishTimeSeconds > 0f ? $"Time: {result.finishTimeSeconds:F2}s" : "Time: N/A";
            }

            baseCashRow?.Set("Base Cash", FormatCash(breakdown.baseCash));
            placementCashRow?.Set("Placement Bonus", FormatCash(breakdown.placementBonusCash));
            cleanCashRow?.Set("Clean Bonus", FormatCash(breakdown.cleanBonusCash));
            penaltiesCashRow?.Set("Penalties", FormatCash(-breakdown.penaltiesCash));

            baseRespectRow?.Set("Base Respect", FormatRespect(breakdown.baseRespect));
            placementRespectRow?.Set("Placement Bonus", FormatRespect(breakdown.placementBonusRespect));
            penaltiesRespectRow?.Set("Penalties", FormatRespect(-breakdown.penaltiesRespect));

            if (totalCashText != null)
            {
                totalCashText.text = FormatCash(breakdown.totalCash);
            }

            if (totalRespectText != null)
            {
                totalRespectText.text = FormatRespect(breakdown.totalRespect);
            }

            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _onContinue = null;
        }

        private void HandleContinueClicked()
        {
            var callback = _onContinue;
            Close();
            callback?.Invoke();
        }

        private static string FormatCash(long amount)
        {
            return $"${amount:N0}";
        }

        private static string FormatRespect(int amount)
        {
            return $"{amount:+#;-#;0} REP";
        }
    }
}
