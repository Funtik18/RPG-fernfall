using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Utils
{
    public static class AsyncDelaying
    {
        public static void DelayCall( float delay, Action callback, CancellationToken token = default )
        {
            DelayedCall( delay, callback, token ).Forget();
        }
        
        public static async UniTask DelayedCall( float delay, Action callback, CancellationToken token = default )
        {
            await UniTask.WaitForSeconds( delay, cancellationToken: token );
            callback?.Invoke();
        }

        public static void DelaySystemCall( float delay, Action callback, CancellationToken token = default )
        {
            Task.Run( () => DelayedSystemCall( delay, callback, token ), token );
        }
        
        public static async Task DelayedSystemCall( float delay, Action callback, CancellationToken token = default )
        {
            await Task.Delay( TimeSpan.FromSeconds( delay ), token );
            callback?.Invoke();
        }
    }
}