using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fire.UI.Scroll
{
    public class ScrollWireframeController : MonoBehaviour
    {
        public RectTransform ListContent;
        public RecordListItem ListItemPrefab;

        public Text DetailHeaderText;
        public RectTransform BodyContent;
        public Button BurnButton;
        public Text BurnButtonLabel;

        private readonly List<ScrollRecordData> _records = new List<ScrollRecordData>();
        private readonly List<RecordListItem> _listItems = new List<RecordListItem>();
        private ScrollRecordData _selected;
        private Font _lineFont;

        private void Start()
        {
            _lineFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            _records.AddRange(ScrollRecordSeedData.GetSample());
            PopulateList();

            if (_records.Count > 0)
            {
                Select(_records[0]);
            }

            BurnButton.onClick.AddListener(OnBurnClicked);
        }

        private void PopulateList()
        {
            foreach (var record in _records)
            {
                var item = Instantiate(ListItemPrefab, ListContent);
                item.Bind(record, Select);
                _listItems.Add(item);
            }
        }

        private void Select(ScrollRecordData record)
        {
            _selected = record;

            for (int i = 0; i < _listItems.Count; i++)
            {
                _listItems[i].SetHighlighted(_records[i] == record);
            }

            RefreshDetail();
        }

        private void RefreshDetail()
        {
            DetailHeaderText.text = $"{_selected.Name} — манор {_selected.Manor} · {_selected.Status} · {_selected.Condition}";

            // Destroy() defers to end-of-frame and only runs in Play mode; without it, rapid
            // synchronous re-selection (e.g. in-editor tooling) leaves old lines stacked up.
            for (int i = BodyContent.childCount - 1; i >= 0; i--)
            {
                var child = BodyContent.GetChild(i).gameObject;
                if (Application.isPlaying) Destroy(child); else DestroyImmediate(child);
            }

            // Dependency is a fact of identity, not an obligation, so it survives burning too.
            if (_selected.Status == ScrollStatus.Dependent)
            {
                AddLine($"Зависит от: {_selected.DependentOn}");
            }

            // Holding/Rent are economic facts that survive burning; WeekWorkDays/Debts/Amercements
            // are the labor obligations & punishments a burned roll erases evidence of.
            AddLine(_selected.Holding);
            AddLine(_selected.Rent);

            if (_selected.Condition != ScrollCondition.Burned)
            {
                AddLine(_selected.WeekWorkDays);
                foreach (var debt in _selected.Debts) AddLine(debt);
                foreach (var amercement in _selected.Amercements) AddLine(amercement);
            }

            BurnButtonLabel.text = _selected.Condition == ScrollCondition.Burned ? "Восстановить (тест)" : "Сжечь";

            LayoutRebuilder.ForceRebuildLayoutImmediate(BodyContent);
        }

        private void AddLine(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var go = new GameObject("Line", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            go.transform.SetParent(BodyContent, false);

            var label = go.GetComponent<Text>();
            label.font = _lineFont;
            label.fontSize = 26;
            label.color = Color.white;
            label.text = "— " + text;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;

            var layoutElement = go.GetComponent<LayoutElement>();
            layoutElement.minHeight = 30;
            layoutElement.flexibleWidth = 1;
        }

        private void OnBurnClicked()
        {
            if (_selected == null) return;

            _selected.Condition = _selected.Condition == ScrollCondition.Burned
                ? ScrollCondition.Intact
                : ScrollCondition.Burned;

            int index = _records.IndexOf(_selected);
            if (index >= 0)
            {
                _listItems[index].Bind(_selected, Select);
                _listItems[index].SetHighlighted(true);
            }

            RefreshDetail();
        }
    }
}
