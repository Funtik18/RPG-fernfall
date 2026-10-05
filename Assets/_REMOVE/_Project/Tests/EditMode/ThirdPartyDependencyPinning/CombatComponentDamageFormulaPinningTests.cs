using NUnit.Framework;
using TurnBasedStrategyFramework.Common.Units;
using Fire.Tests.TestSupport;

namespace Fire.Tests.ThirdPartyDependencyPinning
{
    // Pinning tests, not correctness tests: CombatComponent.CalculateDamageDealt/CalculateDamageTaken
    // and the Health<=0 destruction threshold are TBSFramework's own code (CombatComponent.cs),
    // TestGridUnit does not override any of it - "is TBSF's formula right" is the package
    // maintainer's responsibility, not ours (see CLAUDE.md's TBSFramework-testing rule).
    //
    // These tests exist for a different reason: this exact formula (damage = max(dealt - defence, 1))
    // and the exact Health<=0 destruction threshold are direct, load-bearing inputs to this game's
    // combat balance. If a future TBSFramework package update silently changes either one, we want a
    // red test immediately - not a balance discrepancy discovered by playtesters after release. If
    // this file goes red after a package upgrade, the fix is almost never "correct" TBSF - it's
    // "decide whether the project's balance assumptions still hold, then update the pinned value
    // deliberately."
    public class CombatComponentDamageFormulaPinningTests
    {
        [TestCase(3)]
        [TestCase(0)]
        [TestCase(10)]
        public void CalculateDamageDealt_ReturnsAttackersAttackFactor(int attackFactor)
        {
            var aggressor = new FakeUnit { AttackFactor = attackFactor };
            var defender = new FakeUnit();
            var combat = new CombatComponent(aggressor);

            var dealt = combat.CalculateDamageDealt(defender, defenderCell: null, aggressorCell: null);

            Assert.AreEqual(attackFactor, dealt);
        }

        [TestCase(5f, 2, 3f, TestName = "CalculateDamageTaken_NormalCase_SubtractsDefence")]
        [TestCase(5f, 5, 1f, TestName = "CalculateDamageTaken_DefenceEqualsDealt_ClampsToMinimumOfOne")]
        [TestCase(5f, 8, 1f, TestName = "CalculateDamageTaken_DefenceExceedsDealt_ClampsToMinimumOfOne")]
        [TestCase(5f, -2, 7f, TestName = "CalculateDamageTaken_NegativeDefence_IncreasesDamage")]
        public void CalculateDamageTaken_MatchesMaxDealtMinusDefenceOrOne(float damageDealt, int defenceFactor, float expected)
        {
            var aggressor = new FakeUnit();
            var defender = new FakeUnit { DefenceFactor = defenceFactor };
            var combat = new CombatComponent(defender);

            var taken = combat.CalculateDamageTaken(aggressor, damageDealt, aggressorCell: null, defenderCell: null);

            Assert.AreEqual(expected, taken);
        }

        [Test]
        public void ModifyHealth_HealthDropsToExactlyZero_InvokesDestroyed()
        {
            var unit = new FakeUnit { Health = 5f };
            var attacker = new FakeUnit();
            var combat = new CombatComponent(unit);

            combat.ModifyHealth(-5f, attacker);

            Assert.AreEqual(0f, unit.Health);
            Assert.IsTrue(unit.DestroyedInvoked, "Health reaching exactly 0 must trigger destruction, not just below it.");
        }

        [Test]
        public void ModifyHealth_LethalHitCrossingWellBelowZero_InvokesDestroyed()
        {
            var unit = new FakeUnit { Health = 3f };
            var attacker = new FakeUnit();
            var combat = new CombatComponent(unit);

            combat.ModifyHealth(-99f, attacker);

            Assert.Less(unit.Health, 0f);
            Assert.IsTrue(unit.DestroyedInvoked, "A single hit that overshoots past 0 must still trigger destruction.");
        }

        [Test]
        public void ModifyHealth_NonLethalHit_DoesNotInvokeDestroyed()
        {
            var unit = new FakeUnit { Health = 10f };
            var attacker = new FakeUnit();
            var combat = new CombatComponent(unit);

            combat.ModifyHealth(-3f, attacker);

            Assert.AreEqual(7f, unit.Health);
            Assert.IsFalse(unit.DestroyedInvoked);
        }

        [Test]
        public void ModifyHealth_AlwaysInvokesHealthChangedWithTheRawDelta()
        {
            var unit = new FakeUnit { Health = 10f };
            var attacker = new FakeUnit();
            var combat = new CombatComponent(unit);

            combat.ModifyHealth(-4f, attacker);

            CollectionAssert.Contains(unit.HealthChangeAmounts, -4f);
        }
    }
}
