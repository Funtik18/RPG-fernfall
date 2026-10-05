using NUnit.Framework;
using UnityEngine;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;

namespace Fire.Tests.ThirdPartyDependencyPinning
{
    // Pinning tests, not correctness tests: CinemachineOrbitalFollow.RecenteringTarget and how
    // MutateCameraState uses it are Cinemachine's own code - "is TrackingTarget the right default,
    // does AxisCenter behave this way" is the package maintainer's call, not ours (see CLAUDE.md's
    // TBSFramework-testing rule, which applies equally to any third-party package here).
    //
    // These tests exist because TacticalCameraController.HandleRotation()'s snap-to-diagonal feature
    // is a direct, load-bearing consumer of being able to set HorizontalAxis.Center and have it
    // stick. Cinemachine's default RecenteringTarget (TrackingTarget) silently overwrites Center
    // every frame from the Follow target's forward vector whenever Recentering.Enabled is true,
    // discarding whatever a custom script just assigned - not a Cinemachine bug (it's the documented
    // "dynamic recentering behind a moving target" feature), but it silently broke our snap-to-
    // diagonal logic for a full session before the conflict was found by reading MutateCameraState's
    // source directly. TacticalCameraController.OnEnable() defends against it by forcing
    // RecenteringTarget = AxisCenter. If a future Cinemachine package update changes what either
    // setting does, this should go red immediately - not resurface as "camera always snaps to the
    // same angle" discovered by playtesting months later.
    public class CinemachineOrbitalFollowRecenteringPinningTests
    {
        GameObject _followGo;
        GameObject _camGo;
        CinemachineOrbitalFollow _orbital;

        [SetUp]
        public void SetUp()
        {
            _followGo = new GameObject("PinningTest_Follow");
            _camGo = new GameObject("PinningTest_Cam");
            _camGo.AddComponent<CinemachineCamera>().Follow = _followGo.transform;
            _orbital = _camGo.AddComponent<CinemachineOrbitalFollow>();

            // Matches Test_SyntyOnGrid's TacticalCamera configuration exactly - BindingMode also
            // gates AxisCenter's own behavior (see the second test below), so pin it explicitly
            // rather than relying on whatever Cinemachine's own default happens to be.
            var tracker = _orbital.TrackerSettings;
            tracker.BindingMode = BindingMode.WorldSpace;
            _orbital.TrackerSettings = tracker;

            _orbital.HorizontalAxis.Value = 100f;
            _orbital.HorizontalAxis.Center = 135f; // a custom script's intended snap target
            var recentering = _orbital.HorizontalAxis.Recentering;
            recentering.Enabled = true;
            _orbital.HorizontalAxis.Recentering = recentering;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_camGo);
            Object.DestroyImmediate(_followGo);
        }

        CameraState DefaultStateAtCurrentTransform()
        {
            var state = CameraState.Default;
            state.RawPosition = _camGo.transform.position;
            state.RawOrientation = _camGo.transform.rotation;
            state.ReferenceUp = Vector3.up;
            return state;
        }

        [Test]
        public void MutateCameraState_WithTrackingTargetRecentering_OverwritesCenter()
        {
            _orbital.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget;

            var state = DefaultStateAtCurrentTransform();
            _orbital.MutateCameraState(ref state, 0.016f);

            Assert.AreNotEqual(135f, _orbital.HorizontalAxis.Center,
                "This is the actual bug mechanism: TrackingTarget recomputes Center from the Follow " +
                "target's forward every frame, discarding whatever a custom script set it to.");
        }

        [Test]
        public void MutateCameraState_WithAxisCenterRecentering_LeavesCenterUntouched()
        {
            _orbital.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.AxisCenter;

            var state = DefaultStateAtCurrentTransform();
            _orbital.MutateCameraState(ref state, 0.016f);

            Assert.AreEqual(135f, _orbital.HorizontalAxis.Center,
                "AxisCenter is the setting TacticalCameraController.OnEnable() forces specifically " +
                "so a custom Center assignment survives - if this goes red, Cinemachine changed what " +
                "AxisCenter means and the fix needs re-evaluating, not blind re-pinning.");
        }
    }
}
