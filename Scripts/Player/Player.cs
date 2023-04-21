using Godot;
using System.Linq;

namespace Game
{
    public enum PlayerInputType
    {
        Gamepad,
        Mouse,
        Action,
        MouseDrag
    }

    public class PlayerData
    {
        public PlayerStaticData StaticData { get; set; }

        public PlayerInputType InputType { get; set; }
    }

    public interface IPlayerInputHandler
    {
        bool CanHandle();
    }

    public interface IPlayerGrabEndHandler : IPlayerInputHandler
    {
        void OnGrabEnded();
    }

    public interface IPlayerGrabStartHandler : IPlayerInputHandler
    {
        void OnGrabStarted();
    }

    public interface IPlayerGrabbingHandler : IPlayerInputHandler
    {
        void OnGrabbing(double delta);
    }

    public interface IPlayerClickHandler : IPlayerInputHandler
    {
        void OnClicked();
    }

    public interface IPlayerIdleHandler : IPlayerInputHandler
    {
        void OnIdled(double delta);
    }

    public interface IGrabbable
    {
        public Node2D AsNode2D { get; }
        public void OnGrabStart(Node2D grabber);
        public void OnGrabbing(double delta);
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
        public delegate void ClickedEventHandler();
        /// <summary>
        /// Emitted when the player starts grabbing.
        /// </summary>
        [Signal]
        public delegate void GrabStartedEventHandler();
        /// <summary>
        /// Emitted every frame when the player is grabbing.
        /// </summary>
        [Signal]
        public delegate void GrabbingEventHandler(double delta);
        /// <summary>
        /// Emitted when the player stops grabbing.
        /// </summary>
        [Signal]
        public delegate void GrabEndedEventHandler();

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
        private float angularVelocitySampleInterval = 0.1f;

        [ExportCategory("Dependencies")]
        [Export]
        private NodePath[] clickHandlerNodes = new NodePath[0];
        public IPlayerClickHandler[] ClickHandlers { get; private set; }
        [Export]
        private NodePath[] grabStartHandlerNodes = new NodePath[0];
        public IPlayerGrabStartHandler[] GrabStartHandlers { get; private set; }
        [Export]
        private NodePath[] grabbingHandlerNodes = new NodePath[0];
        public IPlayerGrabbingHandler[] GrabbingHandlers { get; private set; }
        [Export]
        private NodePath[] grabEndHandlerNodes = new NodePath[0];
        public IPlayerGrabEndHandler[] GrabEndHandlers { get; private set; }
        [Export]
        private NodePath[] idleHandlerNodes = new NodePath[0];
        public IPlayerIdleHandler[] IdleHandlers { get; private set; }

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

        public override void _Ready()
        {
            angularVelocityDeltaBuffer = new float[angularVelocitySamples];
            ClickHandlers = clickHandlerNodes.Select(x => GetNode<IPlayerClickHandler>(x)).ToArray();
            GrabStartHandlers = grabStartHandlerNodes.Select(x => GetNode<IPlayerGrabStartHandler>(x)).ToArray();
            GrabbingHandlers = grabbingHandlerNodes.Select(x => GetNode<IPlayerGrabbingHandler>(x)).ToArray();
            GrabEndHandlers = grabEndHandlerNodes.Select(x => GetNode<IPlayerGrabEndHandler>(x)).ToArray();
            IdleHandlers = idleHandlerNodes.Select(x => GetNode<IPlayerIdleHandler>(x)).ToArray();
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
                    foreach (var handler in GrabStartHandlers)
                        if (handler.CanHandle())
                        {
                            handler.OnGrabStarted();
                            break;
                        }
                }

                if (heldTime >= minHeldTime && grabStarted)
                {
                    EmitSignal(nameof(Grabbing), delta);
                    foreach (var handler in GrabbingHandlers)
                        if (handler.CanHandle())
                        {
                            handler.OnGrabbing(delta);
                            break;
                        }
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
                        foreach (var handler in ClickHandlers)
                            if (handler.CanHandle())
                            {
                                handler.OnClicked();
                                break;
                            }
                    }
                    else
                    {
                        // We held for more than the min held time so this was the end of a grab.
                        grabStarted = false;
                        EmitSignal(nameof(GrabEnded));
                        foreach (var handler in GrabEndHandlers)
                            if (handler.CanHandle())
                            {
                                handler.OnGrabEnded();
                                break;
                            }
                    }
                }
            }

            foreach (var handler in IdleHandlers)
                if (handler.CanHandle())
                {
                    handler.OnIdled(delta);
                    break;
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