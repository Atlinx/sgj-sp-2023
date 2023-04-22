using Godot;
using System;
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
        [Signal]
        public delegate void BodyEnteredHandRangeEventHandler(Node2D body);
        [Signal]
        public delegate void BodyExitedHandRangeEventHandler(Node2D body);

        public PlayerData PlayerData { get; set; }
        public float AverageAngularVelocity { get; private set; }
        public float AverageLinearVelocity { get; private set; }
        public Vector2 GrabOffset { get; set; }
        public bool CanMove { get; set; } = true;


        [ExportCategory("Settings")]
        [Export]
        private float minHeldTime = 0.5f;
        [Export]
        private float maxDoubleClickTime = 0.2f;
        private bool disabled = false;
        [Export]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;
                if (disabled)
                {
                    // Make sure to end grab if we already started it.
                    if (grabStarted)
                        EndGrab();
                    ResetInput();
                }
            }
        }

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
        private Node2D spritesContainer;
        [Export]
        private AnimatedSprite2D baseSprite;
        [Export]
        private AnimatedSprite2D colorSprite;
        [Export]
        private Area2D collider;
        [Export]
        private string handOpenAnimation;
        [Export]
        private string handGrabAnimation;
        [Export]
        private string handPointerAnimation;
        [Export]
        public StatusHolder StatusHolder { get; private set; }

        private Whisk whisk = null;
        private IPlayerInput input;
        private bool grabStarted = false;
        private bool prevHeld = false;
        private float heldTime;

        public enum SpriteStateEnum
        {
            Open,
            Grab,
            Point,
        }

        private SpriteStateEnum spriteState;
        public SpriteStateEnum SpriteState
        {
            get => spriteState;
            set
            {
                if (spriteState == value) return;

                spriteState = value;
                var tween = GetTree().CreateTween();
                switch (spriteState)
                {
                    case SpriteStateEnum.Open:
                        baseSprite.Animation = handOpenAnimation;
                        colorSprite.Animation = handOpenAnimation;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1.1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case SpriteStateEnum.Grab:
                        baseSprite.Animation = handGrabAnimation;
                        colorSprite.Animation = handGrabAnimation;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", GrabOffset, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case SpriteStateEnum.Point:
                        baseSprite.Animation = handPointerAnimation;
                        colorSprite.Animation = handPointerAnimation;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                }
            }
        }

        public void Construct(PlayerData playerData, IPlayerInput input)
        {
            PlayerData = playerData;
            this.input = input;
            colorSprite.SelfModulate = playerData.StaticData.Color;
            colorSprite.Play();
            baseSprite.Play();
        }

        public override void _Ready()
        {
            ClickHandlers = clickHandlerNodes.Select(x => GetNode<IPlayerClickHandler>(x)).ToArray();
            GrabStartHandlers = grabStartHandlerNodes.Select(x => GetNode<IPlayerGrabStartHandler>(x)).ToArray();
            GrabbingHandlers = grabbingHandlerNodes.Select(x => GetNode<IPlayerGrabbingHandler>(x)).ToArray();
            GrabEndHandlers = grabEndHandlerNodes.Select(x => GetNode<IPlayerGrabEndHandler>(x)).ToArray();
            IdleHandlers = idleHandlerNodes.Select(x => GetNode<IPlayerIdleHandler>(x)).ToArray();

            collider.AreaEntered += (area) => EmitSignal(nameof(BodyEnteredHandRange), area);
            collider.AreaExited += (area) => EmitSignal(nameof(BodyExitedHandRange), area);
            collider.BodyEntered += (body) => EmitSignal(nameof(BodyEnteredHandRange), body);
            collider.BodyExited += (body) => EmitSignal(nameof(BodyExitedHandRange), body);
        }

        public override void _Process(double delta)
        {
            UpdateInput(delta);
            if (whisk != null)
            {
                AverageAngularVelocity = whisk.AverageAngularVelocity;
                AverageLinearVelocity = whisk.AverageLinearVelocity;
            }
            else
            {
                AverageAngularVelocity = 0;
                AverageLinearVelocity = 0;
            }
            if (CanMove)
                Position = input.PlayerPosition;
        }

        public void SetWhisk(Whisk whisk)
        {
            this.whisk = whisk;
        }

        public Whisk GetWhisk()
        {
            return this.whisk;
        }

        public void RemoveWhisk()
        {
            this.whisk = null;
        }

        private void ResetInput()
        {
            heldTime = 0;
            grabStarted = false;
        }

        private void EndGrab()
        {
            EmitSignal(nameof(GrabEnded));
            foreach (var handler in GrabEndHandlers)
                if (handler.CanHandle())
                {
                    handler.OnGrabEnded();
                    break;
                }
        }

        private void UpdateInput(double delta)
        {
            if (Disabled) return;

            // Block A
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

                if (grabStarted)
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

            // Block B
            if (input.Held != prevHeld)
            {
                prevHeld = input.Held;
                if (!input.Held)
                {
                    // Released
                    if (heldTime < minHeldTime)
                    {
                        // We held for less than the min held time so this was a click
                        ResetInput();
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
                        // Have to reset this here instead of in the next update loop or else Block A will run first and cause bugs
                        ResetInput();
                        EndGrab();
                    }
                }
            }

            foreach (var handler in IdleHandlers)
                if (handler.CanHandle())
                {
                    handler.OnIdled(delta);
                    break;
                }

            var rect = GetCanvasTransform().AffineInverse() * GetViewport().GetVisibleRect();
            input.PlayerPosition = new Vector2(
                Math.Clamp(input.PlayerPosition.X, rect.Position.X, rect.End.X),
                Math.Clamp(input.PlayerPosition.Y, rect.Position.Y, rect.End.Y)
            );
        }
    }
}