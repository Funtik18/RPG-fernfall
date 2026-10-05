using Cysharp.Threading.Tasks;
using Game.Systems.StorageSystem;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerController : IMemento
    {
        public PlayerConfig Config { get; }
        public PlayerObject View { get; }

        private readonly PlayerContext _context;
        private readonly DataHolder _dataHolder;
        
        public PlayerController(
            PlayerConfig config,
            PlayerObject view,
            PlayerContext context,
            DataHolder dataHolder
            )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
            View = view ?? throw new ArgumentNullException( nameof(view) );
            _context = context ?? throw new ArgumentNullException( nameof(context) );
            _dataHolder = dataHolder ?? throw new ArgumentNullException( nameof(dataHolder) );
            
            View.SetController( this );
        }
        
        public void Initialize()
        {
            _context.Initialize();

            RestoreCommit();
            
            Initialization().Forget();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }

        private async UniTask Initialization()
        {
            await UniTask.Yield( PlayerLoopTiming.FixedUpdate );
            await UniTask.Yield( PlayerLoopTiming.FixedUpdate );
            _context.TriggerController.EnableTriggerEnter( true );
            await UniTask.WaitForSeconds( 1f );
            _context.TriggerController.TryEnterLastInteractable();
        }
        
        public void Teleport( Vector3 position )
        {
            View.Rigidbody.position = position;
        }

        public void Commit()
        {
            var data = _dataHolder.GameStorageData.ProgressData.Value.Player;
            data.Position = View.Rigidbody.position;
            data.Rotation = View.Rigidbody.rotation.eulerAngles;
        }

        public void RestoreCommit()
        {
            var data = _dataHolder.GameStorageData.ProgressData.Value.Player;
            View.Rigidbody.position = data.Position;
            View.Rigidbody.rotation = Quaternion.Euler( data.Rotation );
        }
    }
}