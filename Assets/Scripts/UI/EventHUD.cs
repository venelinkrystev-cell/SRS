using UnityEngine;
using UnityEngine.UI;

namespace SRS.UI
{
    public sealed class EventHUD : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text progressText;

        public void Show(string title)
        {
            gameObject.SetActive(true);
            if (titleText != null)
            {
                titleText.text = title;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetTimer(float elapsed)
        {
            if (timerText == null)
            {
                return;
            }

            timerText.text = $"{elapsed:0.0}s";
        }

        public void SetProgress(int checkpointIndex, int checkpointCount, int lap, int lapCount)
        {
            if (progressText == null)
            {
                return;
            }

            if (lapCount > 1)
            {
                progressText.text = $"CP {checkpointIndex}/{checkpointCount} • Lap {lap}/{lapCount}";
                return;
            }

            progressText.text = $"CP {checkpointIndex}/{checkpointCount}";
        }
    }
}
