using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Fire.UI.Scroll;

namespace Fire.Tests.Scroll
{
    public class RecordListItemTests
    {
        private GameObject _root;
        private RecordListItem _item;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("RecordListItem");
            _item = _root.AddComponent<RecordListItem>();

            _item.NameText = CreateText("NameText");
            _item.ManorText = CreateText("ManorText");
            _item.StatusText = CreateText("StatusText");
            _item.ConditionIndicator = CreateImage("ConditionIndicator");
            _item.Background = CreateImage("Background");
            _item.SelectButton = _root.AddComponent<Button>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        private Text CreateText(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_root.transform);
            return go.AddComponent<Text>();
        }

        private Image CreateImage(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_root.transform);
            return go.AddComponent<Image>();
        }

        [Test]
        public void Bind_PopulatesTextFieldsFromRecord()
        {
            var record = new ScrollRecordData
            {
                Name = "Роберт Кок",
                Manor = "Фоббинг",
                Status = ScrollStatus.Cottar
            };

            _item.Bind(record, _ => { });

            Assert.AreEqual("Роберт Кок", _item.NameText.text);
            Assert.AreEqual("Фоббинг", _item.ManorText.text);
            Assert.AreEqual("Cottar", _item.StatusText.text);
        }

        [Test]
        public void Bind_SetsConditionIndicatorColorFromCondition()
        {
            var record = new ScrollRecordData { Condition = ScrollCondition.Burned };

            _item.Bind(record, _ => { });

            Assert.AreEqual(RecordListItem.GetConditionColor(ScrollCondition.Burned), _item.ConditionIndicator.color);
        }

        [Test]
        public void Bind_ClickingSelectButtonInvokesCallbackWithBoundRecord()
        {
            var record = new ScrollRecordData { Name = "Джон с Опушки" };
            ScrollRecordData clicked = null;

            _item.Bind(record, r => clicked = r);
            _item.SelectButton.onClick.Invoke();

            Assert.AreSame(record, clicked);
        }

        [Test]
        public void Bind_CalledTwice_DoesNotStackDuplicateClickListeners()
        {
            var recordA = new ScrollRecordData { Name = "A" };
            var recordB = new ScrollRecordData { Name = "B" };
            int callCount = 0;

            _item.Bind(recordA, _ => callCount++);
            _item.Bind(recordB, _ => callCount++);
            _item.SelectButton.onClick.Invoke();

            Assert.AreEqual(1, callCount, "Re-binding must replace, not stack, the click listener.");
        }

        [TestCase(ScrollCondition.Intact)]
        [TestCase(ScrollCondition.Burned)]
        [TestCase(ScrollCondition.Rewritten)]
        public void GetConditionColor_ReturnsADistinctColorPerCondition(ScrollCondition condition)
        {
            var color = RecordListItem.GetConditionColor(condition);

            var others = System.Enum.GetValues(typeof(ScrollCondition))
                .Cast<ScrollCondition>()
                .Where(c => c != condition);

            foreach (var other in others)
            {
                Assert.AreNotEqual(color, RecordListItem.GetConditionColor(other),
                    $"{condition} and {other} must map to different colors.");
            }
        }

        [Test]
        public void SetHighlighted_TogglesBackgroundColor()
        {
            _item.SetHighlighted(true);
            var highlightedColor = _item.Background.color;

            _item.SetHighlighted(false);
            var unhighlightedColor = _item.Background.color;

            Assert.AreNotEqual(highlightedColor, unhighlightedColor);
        }
    }
}
