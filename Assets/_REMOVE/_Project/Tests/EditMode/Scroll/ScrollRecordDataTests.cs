using System.Linq;
using NUnit.Framework;
using Fire.UI.Scroll;

namespace Fire.Tests.Scroll
{
    public class ScrollRecordDataTests
    {
        [Test]
        public void NewRecord_HasEmptyInitializedListsAndDefaultDependentOn()
        {
            var record = new ScrollRecordData();

            Assert.IsNotNull(record.Debts);
            Assert.IsEmpty(record.Debts);
            Assert.IsNotNull(record.Amercements);
            Assert.IsEmpty(record.Amercements);
            Assert.AreEqual("", record.DependentOn);
        }

        [Test]
        public void GetSample_ReturnsNonEmptyRecordsWithRequiredIdentityFields()
        {
            var records = ScrollRecordSeedData.GetSample();

            Assert.IsNotEmpty(records);
            foreach (var record in records)
            {
                Assert.IsNotEmpty(record.Name, "Every record must have a name.");
                Assert.IsNotEmpty(record.Manor, "Every record must have a manor.");
                Assert.IsNotNull(record.Debts);
                Assert.IsNotNull(record.Amercements);
            }
        }

        [Test]
        public void GetSample_DependentRecord_HasDependentOnSet()
        {
            var records = ScrollRecordSeedData.GetSample();
            var dependent = records.Single(r => r.Status == ScrollStatus.Dependent);

            Assert.IsNotEmpty(dependent.DependentOn);
        }

        [Test]
        public void GetSample_AllRecordsStartIntact()
        {
            var records = ScrollRecordSeedData.GetSample();

            Assert.IsTrue(records.All(r => r.Condition == ScrollCondition.Intact));
        }

        [Test]
        public void GetSample_ReturnsFreshListEachCall()
        {
            var first = ScrollRecordSeedData.GetSample();
            first[0].Condition = ScrollCondition.Burned;

            var second = ScrollRecordSeedData.GetSample();

            Assert.AreEqual(ScrollCondition.Intact, second[0].Condition,
                "Mutating one sample's records must not leak into a freshly requested sample.");
        }
    }
}
