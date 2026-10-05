using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Units;

namespace Fire.Camera
{
    // Cinemachine (CinemachineOrbitalFollow, ThreeRing orbit style) owns the actual camera rig
    // geometry/damping/deocclusion. This script owns only what Cinemachine has no concept of:
    // discrete zoom steps, snap-to-diagonal on rotation release, grid-bounded panning with a
    // terrain-following focus point, and bridging TBSF's unit-selection event into a camera move.
    public class TacticalCameraController : MonoBehaviour
    {
        public enum ZoomLevel { Close = 0, Tactical = 1, Strategic = 2 }

        [Header("Rig")]
        public CinemachineOrbitalFollow OrbitalFollow;
        public Transform Focus;
        public UnityEngine.Camera ViewCamera;
        public UnityUnitManager UnitManager;
        public InputActionAsset InputActions;
        public LayerMask GroundMask;

        [Header("Tuning")]
        public float PanSpeed = 10f;
        public float RotateSpeedDegPerSec = 120f;
        public float MouseRotateDegPerPixel = 0.3f;
        public float ZoomSmoothTime = 0.25f;
        public float RotationSnapTime = 0.35f;
        public float RotateReleaseDeadzone = 0.05f;
        public Vector2 PanBoundsMin = new Vector2(-1.5f, -1.5f);
        public Vector2 PanBoundsMax = new Vector2(9.5f, 9.5f);
        public float RaycastHeight = 50f;

        static readonly float[] ZoomVerticalValue = { 0f, 0.5f, 1f };
        static readonly float[] Diagonals = { 45f, 135f, -135f, -45f };

        ZoomLevel _zoomLevel = ZoomLevel.Tactical;
        float _zoomVelocity;
        bool _wasRotating;

        InputAction _zoomInAction, _zoomOutAction, _rotateAction, _rotateModifierAction, _mouseDeltaAction, _panAction;

        void Awake()
        {
            var map = InputActions.FindActionMap("Camera", true);
            _zoomInAction = map.FindAction("ZoomIn", true);
            _zoomOutAction = map.FindAction("ZoomOut", true);
            _rotateAction = map.FindAction("Rotate", true);
            _rotateModifierAction = map.FindAction("RotateModifier", true);
            _mouseDeltaAction = map.FindAction("MouseDelta", true);
            _panAction = map.FindAction("Pan", true);
            map.Enable();
        }

        void OnEnable()
        {
            // TBSF's UnitManager.Initialize() (which populates GetUnits()) runs from
            // UnityGridController.Start(), which is always after every OnEnable(). Subscribing
            // to UnitAdded here (Awake/OnEnable phase) is what actually catches every unit,
            // including ones that exist at scene load - there is nothing to iterate yet.
            if (UnitManager != null)
                UnitManager.UnitAdded += Subscribe;
            if (OrbitalFollow != null)
            {
                OrbitalFollow.VerticalAxis.Value = ZoomVerticalValue[(int)_zoomLevel];

                // CinemachineOrbitalFollow.MutateCameraState recomputes HorizontalAxis.Center every
                // frame from RecenteringTarget whenever Recentering.Enabled is true - with the
                // Cinemachine default (TrackingTarget), it overwrites whatever NearestDiagonal()
                // sets Center to in HandleRotation, every single frame, so the snap-to-diagonal
                // value we assign is silently discarded and the axis always recenters toward the
                // Follow target's (static) forward instead. AxisCenter is the one setting where
                // OrbitalFollow leaves Center alone and lets us drive it directly. Enforced here
                // rather than only in the scene asset, since re-adding/reconfiguring this component
                // resets RecenteringTarget to its Cinemachine default (TrackingTarget).
                OrbitalFollow.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.AxisCenter;
            }
        }

        void OnDisable()
        {
            if (UnitManager != null)
                UnitManager.UnitAdded -= Subscribe;
        }

        void Subscribe(IUnit unit)
        {
            if (unit is Unit u)
                u.UnitSelected += OnUnitSelected;
        }

        // TBSF's real selection hook: GridStateUnitSelected.OnStateEnter calls
        // IUnit.InvokeUnitSelected(), firing this event. That's the only reliable
        // "a unit was just selected" signal the framework exposes.
        void OnUnitSelected(IUnit unit)
        {
            if (unit?.CurrentCell == null || Focus == null)
                return;
            var wp = unit.CurrentCell.WorldPosition;
            Focus.position = new Vector3(wp.x, wp.y, wp.z);
        }

        void Update()
        {
            HandleZoom();
            HandleRotation();
            HandlePan();
        }

        bool _zoomInWasPressed, _zoomOutWasPressed;

        void HandleZoom()
        {
            // Manual rising-edge detection via IsPressed() rather than WasPerformedThisFrame():
            // the latter is tied to the exact Input System update tick, which is fragile against
            // anything that queues input outside Unity's normal per-frame processing (automated
            // tests included). Edge-detecting our own polled state is robust to both.
            bool zoomInPressed = _zoomInAction != null && _zoomInAction.IsPressed();
            bool zoomOutPressed = _zoomOutAction != null && _zoomOutAction.IsPressed();
            if (zoomInPressed && !_zoomInWasPressed && _zoomLevel < ZoomLevel.Strategic)
                _zoomLevel++;
            if (zoomOutPressed && !_zoomOutWasPressed && _zoomLevel > ZoomLevel.Close)
                _zoomLevel--;
            _zoomInWasPressed = zoomInPressed;
            _zoomOutWasPressed = zoomOutPressed;

            if (OrbitalFollow == null)
                return;
            float target = ZoomVerticalValue[(int)_zoomLevel];
            OrbitalFollow.VerticalAxis.Value = Mathf.SmoothDamp(
                OrbitalFollow.VerticalAxis.Value, target, ref _zoomVelocity, ZoomSmoothTime);
        }

        void HandleRotation()
        {
            if (OrbitalFollow == null)
                return;

            float stickValue = _rotateAction != null ? _rotateAction.ReadValue<float>() : 0f;
            bool modifierHeld = _rotateModifierAction != null && _rotateModifierAction.IsPressed();
            Vector2 mouseDelta = modifierHeld && _mouseDeltaAction != null
                ? _mouseDeltaAction.ReadValue<Vector2>() : Vector2.zero;

            bool isRotating = Mathf.Abs(stickValue) > RotateReleaseDeadzone || modifierHeld;

            if (isRotating)
            {
                float delta = stickValue * RotateSpeedDegPerSec * Time.deltaTime + mouseDelta.x * MouseRotateDegPerPixel;
                OrbitalFollow.HorizontalAxis.Value = OrbitalFollow.HorizontalAxis.ClampValue(
                    OrbitalFollow.HorizontalAxis.Value + delta);
                var r = OrbitalFollow.HorizontalAxis.Recentering;
                r.Enabled = false;
                OrbitalFollow.HorizontalAxis.Recentering = r;
            }
            else if (_wasRotating)
            {
                // Input just released this frame - lock in the nearest diagonal and let
                // Cinemachine's own recentering ease the value there smoothly.
                OrbitalFollow.HorizontalAxis.Center = NearestDiagonal(OrbitalFollow.HorizontalAxis.Value);
                var r = OrbitalFollow.HorizontalAxis.Recentering;
                r.Enabled = true;
                r.Wait = 0f;
                r.Time = RotationSnapTime;
                OrbitalFollow.HorizontalAxis.Recentering = r;
            }
            _wasRotating = isRotating;
        }

        static float NearestDiagonal(float angle)
        {
            float best = Diagonals[0];
            float bestDist = float.MaxValue;
            foreach (var d in Diagonals)
            {
                float dist = Mathf.Abs(Mathf.DeltaAngle(angle, d));
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = d;
                }
            }
            return best;
        }

        void HandlePan()
        {
            if (Focus == null || _panAction == null)
                return;
            Vector2 pan = _panAction.ReadValue<Vector2>();
            if (pan.sqrMagnitude < 0.0001f)
                return;

            Vector3 fwd = Vector3.forward, right = Vector3.right;
            if (ViewCamera != null)
            {
                fwd = Vector3.ProjectOnPlane(ViewCamera.transform.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(ViewCamera.transform.right, Vector3.up).normalized;
            }

            Vector3 move = (fwd * pan.y + right * pan.x) * PanSpeed * Time.deltaTime;
            Vector3 newPos = Focus.position + move;
            newPos.x = Mathf.Clamp(newPos.x, PanBoundsMin.x, PanBoundsMax.x);
            newPos.z = Mathf.Clamp(newPos.z, PanBoundsMin.y, PanBoundsMax.y);
            newPos.y = SampleGroundHeight(newPos);
            Focus.position = newPos;
        }

        // Elevation-aware by construction: on today's flat test grid this always returns 0,
        // but battle maps with height/bridges/stairs will resolve correctly through the same
        // raycast without any change here.
        float SampleGroundHeight(Vector3 worldXZ)
        {
            var origin = new Vector3(worldXZ.x, RaycastHeight, worldXZ.z);
            if (Physics.Raycast(origin, Vector3.down, out var hit, RaycastHeight * 2f, GroundMask))
                return hit.point.y;
            return 0f;
        }
    }
}
