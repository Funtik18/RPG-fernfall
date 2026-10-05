using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Fire.UI.Scroll;

namespace Fire.Tests.Scroll
{
    // ScrollWireframeController wires everything through the Inspector and drives itself from
    // MonoBehaviour.Start(), which the Editor test runner never calls automatically outside Play
    // Mode. These tests build the same hierarchy by hand and invoke Start() via reflection to
    // exercise the real (private) selection/redaction logic instead of duplicating it.
    public class ScrollWireframeControllerTests
    {
        private GameObject _root;
        private ScrollWireframeController _controller;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("ScrollWireframeController");
            _controller = _root.AddComponent<ScrollWireframeController>();

            _controller.ListContent = CreateRectTransform("ListContent");
            _controller.ListItemPrefab = CreateListItemTemplate();
            _controller.DetailHeaderText = CreateText("DetailHeaderText");
            _controller.BodyContent = CreateRectTransform("BodyContent");
            _controller.BurnButton = _root.AddComponent<Button>();
            _controller.BurnButtonLabel = CreateText("BurnButtonLabel");

            InvokeStart();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        private void InvokeStart()
        {
            typeof(ScrollWireframeController)
                .GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_controller, null);
        }

        private RectTransform CreateRectTransform(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_root.transform);
            return go.GetComponent<RectTransform>();
        }

        private Text CreateText(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_root.transform);
            return go.AddComponent<Text>();
        }

        private RecordListItem CreateListItemTemplate()
        {
            var go = new GameObject("RecordListItemTemplate", typeof(RectTransform));
            go.transform.SetParent(_root.transform); // parented so TearDown's DestroyImmediate(_root) also cleans this up
            var item = go.AddComponent<RecordListItem>();
            item.NameText = CreateChildText(go, "NameText");
            item.ManorText = CreateChildText(go, "ManorText");
            item.StatusText = CreateChildText(go, "StatusText");
            item.ConditionIndicator = CreateChildImage(go, "ConditionIndicator");
            item.Background = CreateChildImage(go, "Background");
            item.SelectButton = go.AddComponent<Button>();
            return item;
        }

        private Text CreateChildText(GameObject parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform);
            return go.AddComponent<Text>();
        }

        private Image CreateChildImage(GameObject parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform);
            return go.AddComponent<Image>();
        }

        private string[] BodyLines()
        {
            var lines = new string[_controller.BodyContent.childCount];
            for (int i = 0; i < lines.Length; i++)
                lines[i] = _controller.BodyContent.GetChild(i).GetComponent<Text>().text;
            return lines;
        }

        private RecordListItem ListItemFor(string name)
        {
            for (int i = 0; i < _controller.ListContent.childCount; i++)
            {
                var item = _controller.ListContent.GetChild(i).GetComponent<RecordListItem>();
                if (item.NameText.text == name) return item;
            }
            Assert.Fail($"No list item found for '{name}'.");
            return null;
        }

        [Test]
        public void Start_PopulatesOneListItemPerSeedRecord()
        {
            var seedCount = ScrollRecordSeedData.GetSample().Count;

            Assert.AreEqual(seedCount, _controller.ListContent.childCount);
        }

        [Test]
        public void Start_SelectsFirstRecordByDefault()
        {
            var first = ScrollRecordSeedData.GetSample()[0];

            StringAssert.Contains(first.Name, _controller.DetailHeaderText.text);
            StringAssert.Contains(first.Manor, _controller.DetailHeaderText.text);
        }

        [Test]
        public void Start_IntactRecordShowsAllObligationLines()
        {
            var first = ScrollRecordSeedData.GetSample()[0]; // Villein: Holding, Rent, WeekWorkDays, 3 debts, 1 amercement
            var lines = BodyLines();

            StringAssert.Contains(first.Holding, string.Join("\n", lines));
            StringAssert.Contains(first.Rent, string.Join("\n", lines));
            StringAssert.Contains(first.WeekWorkDays, string.Join("\n", lines));
            foreach (var debt in first.Debts)
                StringAssert.Contains(debt, string.Join("\n", lines));
            foreach (var amercement in first.Amercements)
                StringAssert.Contains(amercement, string.Join("\n", lines));
        }

        [Test]
        public void ClickingListItem_SelectsThatRecordAndRefreshesDetail()
        {
            var dependent = ScrollRecordSeedData.GetSample().Single(r => r.Status == ScrollStatus.Dependent);

            ListItemFor(dependent.Name).SelectButton.onClick.Invoke();

            StringAssert.Contains(dependent.Name, _controller.DetailHeaderText.text);
        }

        [Test]
        public void BurningSelectedRecord_HidesLaborObligationsButKeepsIdentityFacts()
        {
            var first = ScrollRecordSeedData.GetSample()[0];

            _controller.BurnButton.onClick.Invoke();
            var joined = string.Join("\n", BodyLines());

            StringAssert.Contains(first.Holding, joined, "Holding is an identity/economic fact and must survive burning.");
            StringAssert.Contains(first.Rent, joined, "Rent is an identity/economic fact and must survive burning.");
            StringAssert.DoesNotContain(first.WeekWorkDays, joined, "WeekWorkDays is a labor obligation and must be redacted by burning.");
            foreach (var debt in first.Debts)
                StringAssert.DoesNotContain(debt, joined, "Debts must be redacted by burning.");
            foreach (var amercement in first.Amercements)
                StringAssert.DoesNotContain(amercement, joined, "Amercements must be redacted by burning.");
        }

        [Test]
        public void BurningSelectedRecord_UpdatesBurnButtonLabelAndListIndicator()
        {
            _controller.BurnButton.onClick.Invoke();

            Assert.AreEqual("Восстановить (тест)", _controller.BurnButtonLabel.text);

            var first = ScrollRecordSeedData.GetSample()[0];
            var indicatorColor = ListItemFor(first.Name).ConditionIndicator.color;
            Assert.AreEqual(RecordListItem.GetConditionColor(ScrollCondition.Burned), indicatorColor);
        }

        [Test]
        public void BurningThenRestoring_BringsBackAllObligationLines()
        {
            var first = ScrollRecordSeedData.GetSample()[0];

            _controller.BurnButton.onClick.Invoke(); // burn
            _controller.BurnButton.onClick.Invoke(); // restore

            var joined = string.Join("\n", BodyLines());
            Assert.AreEqual("Сжечь", _controller.BurnButtonLabel.text);
            StringAssert.Contains(first.WeekWorkDays, joined);
            foreach (var debt in first.Debts)
                StringAssert.Contains(debt, joined);
        }

        [Test]
        public void BurningDependentRecord_KeepsDependentOnFactVisible()
        {
            var dependent = ScrollRecordSeedData.GetSample().Single(r => r.Status == ScrollStatus.Dependent);
            ListItemFor(dependent.Name).SelectButton.onClick.Invoke();

            _controller.BurnButton.onClick.Invoke();
            var joined = string.Join("\n", BodyLines());

            StringAssert.Contains(dependent.DependentOn, joined,
                "Dependency is a fact of identity, not an obligation, and must survive burning.");
        }
    }
}
