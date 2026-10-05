using UnityEngine;

namespace Game.Core.Gameplay.TBS.Editor
{
    internal static class GridEditorPalette
    {
        public static readonly Color DefaultControl = Color.white;
        public static readonly Color HiddenHandle = Color.clear;

        public static readonly Color ConnectionReachable = new( 0.1f, 0.85f, 0.25f, 1.0f );
        public static readonly Color ConnectionBlocked = new( 1.0f, 0.15f, 0.1f, 1.0f );

        public static readonly Color ClearAction = new( 0.8f, 0.25f, 0.25f );

        public const float MarkerFillAlpha = 0.35f;
        public static readonly Color MarkerLabel = Color.mediumBlue;

        public static readonly Color UnitForwardAxis = Color.blue;
        public static readonly Color UnitRightAxis = Color.red;
        public static readonly Color UnitUpAxis = Color.green;

        public static readonly Color EffectAvailableFill = new( 0.1f, 0.65f, 1.0f, 0.28f );
        public static readonly Color EffectAvailableOutline = new( 0.18f, 0.55f, 0.95f );
        public static readonly Color EffectExistingFill = new( 0.15f, 0.95f, 0.35f, 0.38f );
        public static readonly Color EffectExistingOutline = Color.green;

        public static readonly Color GridLink = new( 0.1f, 0.9f, 0.25f, 1.0f );
    }
}
