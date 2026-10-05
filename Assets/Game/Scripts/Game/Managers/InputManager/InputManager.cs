using UnityEngine;

namespace Game.Managers.InputManager
{
    public static class InputManager
    {
        public static Inputs Inputs { get; private set; }

        public static void Initialize()
        {
            Inputs = new Inputs();
            Inputs.Enable();
        }

        public static void Dispose()
        {
            Inputs?.Disable();
            Inputs?.Dispose();
            Inputs = null;
        }

        public static Vector2 GetPoint() => Inputs.Camera.Point.ReadValue< Vector2 >();
    }
}