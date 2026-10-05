using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Common.Utilities;

namespace Fire.Tests.TestSupport
{
    // IUnit is a large interface (IMoveable + ICombatant + selection/ability/turn plumbing) because
    // it's TBSF's single unit contract - but AttackZoneCalculator and the CombatComponent pinning
    // tests only ever touch CurrentCell/AttackRange/Health/AttackFactor/DefenceFactor plus the
    // Invoke*/Modify* calls CombatComponent makes directly. Those members carry real, mutable
    // backing state; everything else below is a trivial default (empty/no-op/false) since none of
    // it is exercised by the code these fakes exist to test - this is not a general-purpose IUnit
    // double, just enough surface to compile against the interface.
    public class FakeUnit : IUnit
    {
        public ICell CurrentCell { get; set; }
        public IVector3 WorldPosition { get; set; }
        public float MaxActionPoints { get; set; }
        public float ActionPoints { get; set; }
        public int PlayerNumber { get; set; }
        public ITreeNode BehaviourTree => null;
        public int UnitID { get; set; }

        public float MovementPoints { get; set; }
        public float MaxMovementPoints { get; set; }
        public float MovementAnimationSpeed { get; set; }

        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public int AttackRange { get; set; }
        public int AttackFactor { get; set; }
        public int DefenceFactor { get; set; }

        // Populated by InvokeDestroyed/InvokeHealthChanged so tests can assert on what
        // CombatComponent actually invoked, without needing a real event subscriber.
        public bool DestroyedInvoked { get; private set; }
        public UnitDestroyedEventArgs? LastDestroyedArgs { get; private set; }
        public List<float> HealthChangeAmounts { get; } = new List<float>();

#pragma warning disable CS0067 // interface events that no test in this project currently subscribes to
        public event Action<IUnit> UnitSelected;
        public event Action<IUnit> UnitDeselected;
        public event Action<IUnit> UnitClicked;
        public event Action<IUnit> UnitHighlighted;
        public event Action<IUnit> UnitDehighlighted;
        public event Action<AbilityUsedEventArgs> AbilityUsed;
        public event Action<UnitMovedEventArgs> UnitMoved;
        public event Action<UnitChangedGridPositionEventArgs> UnitLeftCell;
        public event Action<UnitChangedGridPositionEventArgs> UnitEnteredCell;
        public event Action<UnitPositionChangedEventArgs> UnitWorldPositionChanged;
        public event Action<UnitAttackedEventArgs> UnitAttacked;
        public event Action<UnitDestroyedEventArgs> UnitDestroyed;
        public event Action<HealthChangedEventArgs> HealthChanged;
#pragma warning restore CS0067

        public void InvokeUnitSelected() { }
        public void InvokeUnitDeselected() { }
        public void InvokeUnitClicked() { }
        public void InvokeUnitHighlighted() { }
        public void InvokeUnitDehighlighted() { }
        public void InvokeAbilityUsed(AbilityUsedEventArgs args) { }
        public void InvokeUnitMoved(UnitMovedEventArgs eventArgs) { }
        public void InvokeUnitLeftCell(UnitChangedGridPositionEventArgs eventArgs) { }
        public void InvokeUnitEnteredCell(UnitChangedGridPositionEventArgs eventArgs) { }
        public void InvokeUnitPositionChanged(UnitPositionChangedEventArgs eventArgs) { }
        public void InvokeAttacked(UnitAttackedEventArgs eventArgs) { }

        public void InvokeDestroyed(UnitDestroyedEventArgs eventArgs)
        {
            DestroyedInvoked = true;
            LastDestroyedArgs = eventArgs;
        }

        public void InvokeHealthChanged(HealthChangedEventArgs eventArgs)
        {
            HealthChangeAmounts.Add(eventArgs.HealthChangeAmount);
        }

        public bool IsCellMovableTo(ICell destination) => false;
        public bool IsCellTraversable(ICell source, ICell destination) => false;
        public float GetMovementCost(ICell source, ICell destination) => 0f;
        public IEnumerable<ICell> GetAvailableDestinations(IEnumerable<ICell> cells) => Enumerable.Empty<ICell>();
        public IEnumerable<ICell> FindPath(ICell destination, ICellManager cellManager) => Enumerable.Empty<ICell>();
        public Dictionary<ICell, Dictionary<ICell, float>> GetGraphEdges(ICellManager cellManager) => new Dictionary<ICell, Dictionary<ICell, float>>();
        public void CachePaths(ICellManager cellManager) { }
        public void InvalidateCache() { }
        public Task MovementAnimation(IEnumerable<ICell> path, ICell destination) => Task.CompletedTask;

        public void ModifyHealth(float healthChangeAmount, IUnit sourceUnit) { }
        public bool IsUnitAttackable(IUnit otherUnit, ICell otherUnitCell, ICell attackSourceCell) => false;
        public float CalculateDamageDealt(IUnit defender, ICell defenderCell, ICell aggressorCell) => AttackFactor;
        public float CalculateDamageDealt(IUnit defender) => AttackFactor;
        public float CalculateDamageTaken(IUnit aggressor, float damageDealt, ICell aggressorCell, ICell defenderCell) => Math.Max(damageDealt - DefenceFactor, 1);
        public float CalculateDamageTaken(IUnit aggressor, float damageDealt) => Math.Max(damageDealt - DefenceFactor, 1);
        public float CalculateTotalDamage(IUnit defender, ICell defenderCell, ICell aggressorCell) => 0f;
        public float CalculateTotalDamage(IUnit defender) => 0f;

        public void Initialize(IGridController gridController) { }
        public IEnumerable<IAbility> GetBaseAbilities() => Enumerable.Empty<IAbility>();
        public void RegisterAbility(IAbility ability, IGridController gridController) { }
        public Task ExecuteAbility(ICommand command, Func<IGridController, Task> preAction, Func<IGridController, Task> postAction, bool isNetworkInvoked = false) => Task.CompletedTask;
        public Task HumanExecuteAbility(ICommand command, IGridController gridController, bool isNetworkInvoked = false) => Task.CompletedTask;
        public Task HumanExecuteAbility(ICommand command, IGridController gridController, Func<IGridController, Task> preAction, Func<IGridController, Task> postAction, bool isNetworkInvoked = false) => Task.CompletedTask;
        public Task AIExecuteAbility(ICommand command, IGridController gridController, TaskCompletionSource<bool> tcs, bool isNetworkInvoked = false) => Task.CompletedTask;
        public Task AIExecuteAbility(ICommand command, IGridController gridController, TaskCompletionSource<bool> tcs, Func<IGridController, Task> preAction, Func<IGridController, Task> postAction, bool isNetworkInvoked = false) => Task.CompletedTask;
        public void OnTurnStart(IGridController gridController) { }
        public void OnTurnEnd(IGridController gridController) { }
        public void Cleanup(IGridController gridController) { }
        public void OnDestroyed(IGridController gridController) { }
        public void RemoveFromGame() { }
    }
}
