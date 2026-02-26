using UnityEngine;
using UnityEngine.UI;

namespace SRS.UI
{
    public sealed class ResultsBreakdownRow : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private Text value;

        public void Set(string rowLabel, string rowValue)
        {
            if (label != null)
            {
                label.text = rowLabel;
            }

            if (value != null)
            {
                value.text = rowValue;
            }
        }
    }
}
