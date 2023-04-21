using Godot;

namespace Game
{
    public enum PlayerInputType
    {
        Gamepad,
        Mouse,
        Action
    }

    public class PlayerData
    {
        public PlayerStaticData StaticData { get; set; }

        public PlayerInputType InputType { get; set; }
    }

    public interface IGrabEndHandler
    {
        public bool CanHandle();
        public void OnGrabEnd();
    }

    public interface IGrabStartHandler
    {
        public bool CanHandle();
        public void OnGrabStart();
    }

    public interface IGrabbingHandler
    {
        public bool CanHandle();
        public void OnGrabbing(double delta);
    }

    public abstract partial class ClickHandler : Node
    {
        public abstract bool CanHandle();
        public abstract void OnClick();
    }

    public interface IGrabbable
    {
        public Node2D AsNode2D { get; }
        public bool IsFixed { get; }
        public void OnGrabStart();
        public void OnGrabEnd();
    }

    public partial class Player : Node2D
    {
        // Possible Interactions Timeline:
        //
        // Timeline represents heldTime increasing after held is first pressed.
        // 
        // LEGEND:
        // * = held start
        // X = held released
        // | = minHeldTime
        //
        //     .-Click
        // *---X-------|----------
        //
        //             .-GrabStarted         .-GrabEnded
        // *-----------|---------------------X------------

        /// <summary>
        /// Emitted when the player clicks.
        /// </summary>
        [Signal]
        public delegate void Clicked();
        /// <summary>
        /// Emitted when the player starts grabbing.
        /// </summary>
        [Signal]
        public delegate void GrabStarted();
        /// <summary>
        /// Emitted when the player stops grabbing.
        /// </summary>
        [Signal]
        public delegate void GrabEnded();

        /// <summary>
        /// True if the player is currently is grabbing.
        /// </summary>
        public bool Grabbing { get; set; }
        public PlayerData PlayerData { get; set; }

        [ExportCategory("Settings")]
        [Export]
        public float AverageAngularVelocity { get; private set; }
        [Export]
        private float minHeldTime = 0.5f;
        [Export]
        private float maxDoubleClickTime = 0.2f;
        [Export]
        private int angularVelocitySamples = 30;
        [Export]
        private float angularVelocitySampleInterval = 0;

        [ExportCategory("Dependencies")]
        [Export]
        private ClickHandler[] clickHandlers;
        [Export]
        private IGrabStartHandler[] grabStartHandlers;
        [Export]
        private IGrabbingHandler[] grabbingHandlers;
        [Export]
        private IGrabEndHandler[] grabEndHandlers;
        [Export]
        private Whisk whisk;
        [Export]
        private Hand hand;

        private IPlayerInput input;
        private Node2D centerPoint;
        private bool grabStarted = false;
        private bool prevHeld = false;
        private float heldTime;
        private float prevAngle;
        private float[] angularVelocityDeltaBuffer;
        private int angularVelocityCurrIndex = 0;
        private float angularVelocitySampleTime = 0;

        public void Construct(PlayerData playerData, Node2D centerPoint, IPlayerInput input)
        {
            PlayerData = playerData;
            this.centerPoint = centerPoint;
            this.input = input;
            whisk.Construct(playerData.StaticData);
            hand.Construct(playerData.StaticData, input);
        }

        public override void _Process(double delta)
        {
            UpdateInput(delta);
            UpdateAverageAngularVelocity(delta);
        }

        private void UpdateInput(double delta)
        {
            if (input.Held)
            {
                heldTime += (float)delta;
                if (heldTime >= minHeldTime && !grabStarted)
                {
                    // We held for more than the min held time so this is a drag.
                    grabStarted = true;
                    EmitSignal(nameof(GrabStarted));
                }
            }

            if (input.Held != prevHeld)
            {
                prevHeld = input.Held;
                if (input.Held)
                {
                    // Pressed
                    heldTime = 0;
                    grabStarted = false;
                }
                else
                {
                    // Released
                    if (heldTime < minHeldTime)
                    {
                        // We held for less than the min held time so this was a click
                        EmitSignal(nameof(Clicked));
                    }
                    else
                        // We held for more than the min held time so this was the end of a grab.
                        EmitSignal(nameof(GrabEnded));
                }
            }
        }

        private void UpdateAverageAngularVelocity(double delta)
        {
            angularVelocitySampleTime += (float)delta;
            while (angularVelocitySampleTime > angularVelocitySampleInterval)
            {
                angularVelocitySampleTime -= angularVelocitySampleInterval;

                Vector2 direction = whisk.Position - centerPoint.Position;
                float newAngle = Mathf.Atan2(direction.Y, direction.X);


                float angleDelta = newAngle - prevAngle;
                // Pi to -pi
                // Delta should still be positive
                if (prevAngle > Mathf.Pi / 2 && newAngle < -Mathf.Pi / 2)
                    angleDelta = newAngle + Mathf.Tau - prevAngle;
                // -Pi to pi
                // Delta should still be negative
                else if (prevAngle < -Mathf.Pi / 2 && newAngle > Mathf.Pi / 2)
                    angleDelta = newAngle - Mathf.Tau - prevAngle;

                prevAngle = newAngle;

                float angleVelocity = angleDelta / (float)delta;

                angularVelocityDeltaBuffer[angularVelocityCurrIndex++] = angleVelocity;
                if (angularVelocityCurrIndex >= angularVelocityDeltaBuffer.Length)
                    angularVelocityCurrIndex = 0;

                float sum = 0;
                foreach (float i in angularVelocityDeltaBuffer)
                    sum += i;

                AverageAngularVelocity = sum / angularVelocityDeltaBuffer.Length;
            }
        }
    }
}