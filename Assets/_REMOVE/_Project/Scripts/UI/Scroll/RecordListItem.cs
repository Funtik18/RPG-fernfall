using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fire.UI.Scroll
{
    public class RecordListItem : MonoBehaviour
    {
        public Text NameText;
        public Text ManorText;
        public Text StatusText;
        public Image ConditionIndicator;
        public Button SelectButton;
        public Image Background;

        private ScrollRecordData _record;
        private Action<ScrollRecordData> _onClicked;

        public void Bind(ScrollRecordData record, Action<ScrollRecordData> onClicked)
        {
            _record = record;
            _onClicked = onClicked;

            NameText.text = record.Name;
            ManorText.text = record.Manor;
            StatusText.text = record.Status.ToString();
            ConditionIndicator.color = GetConditionColor(record.Condition);

            SelectButton.onClick.RemoveAllListeners();
            SelectButton.onClick.AddListener(() => _onClicked?.Invoke(_record));
        }

        public void SetHighlighted(bool highlighted)
        {
            Background.color = highlighted ? new Color(0.35f, 0.35f, 0.4f) : new Color(0.25f, 0.25f, 0.25f);
        }

        public static Color GetConditionColor(ScrollCondition condition)
        {
            switch (condition)
            {
                case ScrollCondition.Burned:
                    return new Color(0.5f, 0.1f, 0.1f);
                case ScrollCondition.Rewritten:
                    return new Color(0.7f, 0.6f, 0.1f);
                default:
                    return new Color(0.75f, 0.75f, 0.75f);
            }
        }
    }
}
