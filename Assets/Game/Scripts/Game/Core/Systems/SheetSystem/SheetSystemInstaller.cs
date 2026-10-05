using UnityEngine;
using Zenject;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "SheetSystemInstaller", menuName = "Installers/SheetSystemInstaller" ) ]
    public sealed class SheetSystemInstaller : ScriptableObjectInstaller< SheetSystemInstaller >
    {
        [ SerializeField ] private InventoryDatabase _database;
        [ SerializeField ] private WeaponRules _weaponRules;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _database );
            Container.BindInstance( _weaponRules );
            
            Container.Bind< EffectFactory >().AsSingle().Lazy();
            Container.Bind< SkillFactory >().AsSingle().Lazy();
            Container.Bind< SheetFactory >().AsSingle().Lazy();
        }
    }
}
