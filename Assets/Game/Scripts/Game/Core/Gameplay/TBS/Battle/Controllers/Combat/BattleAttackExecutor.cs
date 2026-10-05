using Cysharp.Threading.Tasks;
using Game.Core.Systems.SheetSystem;
using Game.VFX;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleAttackExecutor
    {
        public event Action< UnitController, UnitController, float, float > OnDamageApplied;
        public event Action< UnitController, UnitController > OnUnitKilled;

        private readonly BattleGridController _gridController;
        private readonly BattleCombatTargetingController _targetingController;
        private readonly VFXFloatingTextObjectService _vfxFloatingTextObjectService;
        private readonly WeaponRules _weaponRules;

        public BattleAttackExecutor(
            BattleGridController gridController,
            BattleCombatTargetingController targetingController,
            VFXFloatingTextObjectService vfxFloatingTextObjectService,
            WeaponRules weaponRules
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _targetingController = targetingController ?? throw new ArgumentNullException( nameof(targetingController) );
            _vfxFloatingTextObjectService = vfxFloatingTextObjectService ?? throw new ArgumentNullException( nameof(vfxFloatingTextObjectService) );
            _weaponRules = weaponRules;
        }

        public async UniTask ExecuteAttack( UnitController attacker, UnitController defender, bool spendAttack, bool defenderCanCounterattack, CombatArt combatArt = null )
        {
            if ( attacker == defender ) return;
            if ( spendAttack && !attacker.CanAttack ) return;
            if ( !attacker.IsAlive() ) return;
            if ( !defender.IsAlive() ) return;

            if ( spendAttack )
            {
                attacker.SpendAttack();
            }

            await attacker.Movement.FaceTowards( defender.View.transform.position );

            combatArt?.OnBeforeAttack( attacker, defender );
            await attacker.Animator.PlayAttack();
            await ApplyAttackDamage( attacker, defender, defenderCanCounterattack, combatArt );
            combatArt?.OnAfterAttack( attacker, defender );

            SpendWeaponUse( attacker );
        }

        public async UniTask< bool > ApplyAdditionalAttackDamage( UnitController attacker, UnitController defender, bool defenderCanCounterattack )
        {
            if ( attacker == defender ) return false;
            if ( !attacker.IsAlive() ) return false;
            if ( !defender.IsAlive() ) return false;

            return await ApplyAttackDamage( attacker, defender, defenderCanCounterattack, null );
        }

        public async UniTask ExecuteSupport( UnitController attacker, UnitController target )
        {
            if ( attacker == target ) return;
            if ( !attacker.CanAttack ) return;
            if ( !attacker.IsAlive() ) return;
            if ( !target.IsAlive() ) return;

            attacker.SpendAttack();

            await attacker.Movement.FaceTowards( target.View.transform.position );
            await attacker.Animator.PlayAttack();
            ApplySupport( attacker, target );
            SpendWeaponUse( attacker );
        }

        private async UniTask< bool > ApplyAttackDamage( UnitController attacker, UnitController defender, bool defenderCanCounterattack, CombatArt combatArt )
        {
            var isHit = await ApplyDamage( attacker, defender, defenderCanCounterattack );
            if ( isHit && combatArt != null )
            {
                await combatArt.Apply( attacker, defender );
            }
            if ( isHit && defender.IsAlive() && (combatArt == null || !combatArt.ReplacesDefaultDamageReaction) )//ShouldPlayDefaultDamageReaction
            {
                await defender.Animator.PlayDamage();
            }

            return isHit;
        }

        private async UniTask< bool > ApplyDamage( UnitController attacker, UnitController defender, bool defenderCanCounterattack )
        {
            if ( !attacker.IsAlive() ) return false;
            if ( !defender.IsAlive() ) return false;

            attacker.Skills.OnBeforeHit( defender, defenderCanCounterattack, _targetingController.IsAttackFromDistance( attacker, defender ) );

            _gridController.Registry.TryGetUnitCell( defender, out var defenderCell );
            if ( !BattleCombatFormulas.IsHit( attacker, defender, _weaponRules, defenderCell ) )
            {
                _vfxFloatingTextObjectService.DoMissHitText( "Miss", defender.View.transform.position );

                attacker.Skills.OnAfterHit( defender, false );
                await defender.Animator.PlayDodge();
                return false;
            }

            var isCritical = BattleCombatFormulas.IsCriticalHit( attacker, defender );
            var previousHealthPercent = defender.Sheet.Stats.HealthPoints.PercentValue;
            var damage = BattleCombatFormulas.GetDamage( attacker, defender, isCritical, _weaponRules );
            defender.Sheet.Stats.HealthPoints.Value -= damage;
            var currentHealthPercent = defender.Sheet.Stats.HealthPoints.PercentValue;

            attacker.Skills.OnAfterHit( defender, true );
            ApplyOnHitEffects( attacker, defender );

            if ( isCritical )
            {
                _vfxFloatingTextObjectService.DoCritDamageText( "Crit", defender.View.transform.position );
            }

            _vfxFloatingTextObjectService.DoDamageText( damage.ToString(), defender.View.transform.position );
            OnDamageApplied?.Invoke( attacker, defender, previousHealthPercent, currentHealthPercent );

            if ( defender.Sheet.IsDead )
            {
                OnUnitKilled?.Invoke( attacker, defender );
                await Kill( defender );
                return true;
            }

            return true;
        }

        public async UniTask Kill( UnitController unit )
        {
            if ( !unit.View.gameObject.activeInHierarchy ) return;

            _gridController.Registry.Remove( unit );
            unit.Death();
            await unit.Animator.PlayDeath();
            // unit.View.gameObject.SetActive( false );
        }

        private void ApplySupport( UnitController attacker, UnitController target )
        {
            if ( !attacker.IsAlive() ) return;
            if ( !target.IsAlive() ) return;

            attacker.Skills.OnBeforeHit( target, false, _targetingController.IsAttackFromDistance( attacker, target ) );
            attacker.Skills.OnAfterHit( target, true );
            ApplyOnHitEffects( attacker, target );
        }

        private void ApplyOnHitEffects( UnitController attacker, UnitController target )
        {
            var effects = attacker.Sheet.Equipment.Weapon.Config.OnHitEffects;
            foreach ( var effect in effects )
            {
                target.Effects.Apply( effect );
            }
        }

        private void SpendWeaponUse( UnitController attacker )
        {
            var weapon = attacker.Sheet.Equipment.Weapon;
            if ( weapon == null ) return;
            if ( !weapon.SpendUse() ) return;

            if ( attacker.Sheet.Equipment.Weapon == weapon && !weapon.CanUse )
            {
                attacker.Sheet.Equipment.UnequipWeapon();
            }

            if ( !weapon.IsBroken ) return;

            var item = attacker.Sheet.Inventory.GetItem( weapon.Config );
            attacker.Sheet.Inventory.RemoveItem( item );
        }
    }
}
