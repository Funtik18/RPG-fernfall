using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Unity.Cinemachine;
using Fire.Camera;

namespace Fire.Tests.Camera
{
    // NearestDiagonal is a private static helper with no Cinemachine/scene dependency, so it's
    // exercised directly via reflection instead of standing up a full orbital rig for a math check.
    public class TacticalCameraControllerTests
    {
        private static float NearestDiagonal(float angle)
        {
            var method = typeof(TacticalCameraController).GetMethod(
                "NearestDiagonal", BindingFlags.NonPublic | BindingFlags.Static);
            return (float)method.Invoke(null, new object[] { angle });
        }

        [TestCase(50f, 45f)]
        [TestCase(40f, 45f)]
        [TestCase(100f, 135f)]
        [TestCase(170f, 135f)]
        [TestCase(-100f, -135f)]
        [TestCase(-170f, -135f)]
        [TestCase(-50f, -45f)]
        [TestCase(-10f, -45f)]
        public void NearestDiagonal_SnapsToClosestOfFourDiagonals(float input, float expected)
        {
            Assert.AreEqual(expected, NearestDiagonal(input));
        }

        [TestCase(45f)]
        [TestCase(135f)]
        [TestCase(-135f)]
        [TestCase(-45f)]
        public void NearestDiagonal_ExactDiagonalMapsToItself(float exact)
        {
            Assert.AreEqual(exact, NearestDiagonal(exact));
        }

        // Exact midpoints between two diagonals are a genuine tie (both candidates at the same
        // angular distance) - NearestDiagonal breaks ties by iteration order over its Diagonals
        // array ({45, 135, -135, -45}) via a strict "<" comparison, so whichever of the two tied
        // diagonals comes first in that array wins. Derived by hand from the algorithm, not guessed:
        // 90 is equidistant from 45 and 135 -> 45 (checked first); -90 is equidistant from -135 and
        // -45 -> -135 (checked first, at index 2, before -45 at index 3); 0 is equidistant from 45
        // and -45 -> 45 (checked first, at index 0, before -45 at index 3); 180 is equidistant from
        // 135 and -135 -> 135 (checked first, at index 1, before -135 at index 2).
        [TestCase(90f, 45f)]
        [TestCase(-90f, -135f)]
        [TestCase(0f, 45f)]
        [TestCase(180f, 135f)]
        public void NearestDiagonal_ExactMidpointBetweenTwoDiagonals_ResolvesDeterministically(float midpoint, float expected)
        {
            Assert.AreEqual(expected, NearestDiagonal(midpoint));
        }

        [TestCase(TacticalCameraController.ZoomLevel.Close, 0f)]
        [TestCase(TacticalCameraController.ZoomLevel.Tactical, 0.5f)]
        [TestCase(TacticalCameraController.ZoomLevel.Strategic, 1f)]
        public void ZoomVerticalValue_HasExpectedValuePerZoomLevel(TacticalCameraController.ZoomLevel level, float expected)
        {
            var field = typeof(TacticalCameraController).GetField(
                "ZoomVerticalValue", BindingFlags.NonPublic | BindingFlags.Static);
            var table = (float[])field.GetValue(null);

            Assert.AreEqual(expected, table[(int)level]);
        }

        // Regression guard for the "camera always snaps to the same diagonal" bug: Cinemachine's own
        // RecenteringTarget default (TrackingTarget) silently overwrites HorizontalAxis.Center every
        // frame, discarding whatever HandleRotation's NearestDiagonal() just set it to - see
        // CinemachineOrbitalFollowRecenteringPinningTests for the third-party mechanism itself. This
        // test only pins OUR OWN defensive line in OnEnable(), not Cinemachine's behavior, so it
        // belongs here rather than in the ThirdPartyDependencyPinning folder.
        //
        // OnEnable() is invoked directly via reflection - same reasoning as NearestDiagonal above,
        // just applied to a private Unity message method instead of a private static helper.
        // Confirmed by direct experiment that this project's Editor/EditMode context does NOT
        // synchronously fire Awake/OnEnable from GameObject.SetActive(true) the way Play Mode does
        // (both stayed false immediately after SetActive(true) on a throwaway probe component) -
        // going through the real Unity lifecycle here would make this test non-deterministic
        // (or simply never observe the effect at all) rather than test OnEnable's own logic.
        [Test]
        public void OnEnable_ForcesRecenteringTargetToAxisCenter()
        {
            var followGo = new GameObject("Test_Focus");
            var camGo = new GameObject("Test_Cam");
            camGo.AddComponent<CinemachineCamera>().Follow = followGo.transform;
            var orbital = camGo.AddComponent<CinemachineOrbitalFollow>();
            orbital.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget; // Cinemachine's own default

            var controllerGo = new GameObject("Test_Controller");
            var controller = controllerGo.AddComponent<TacticalCameraController>();
            controller.OrbitalFollow = orbital;

            try
            {
                var onEnable = typeof(TacticalCameraController).GetMethod(
                    "OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
                onEnable.Invoke(controller, null);

                Assert.AreEqual(CinemachineOrbitalFollow.ReferenceFrames.AxisCenter, orbital.RecenteringTarget,
                    "OnEnable must force AxisCenter - TrackingTarget (Cinemachine's default) silently " +
                    "overwrites Center every frame, breaking HandleRotation's snap-to-diagonal logic.");
            }
            finally
            {
                Object.DestroyImmediate(controllerGo);
                Object.DestroyImmediate(camGo);
                Object.DestroyImmediate(followGo);
            }
        }
    }
}
