using Godot;

namespace Game
{
    public interface IGrabbable
    {
        event Grabbable.GrabCancelledEventHandler GrabCancelled;
        public bool CanGrab { get; }
        public Node2D AsNode2D { get; }
        public Vector2 GrabOffset { get; }
        public void OnGrabStart(Node2D grabber);
        public void OnGrabbing(double delta);
        public void OnGrabEnd();
    }

    public partial class Grabbable : Node, IGrabbable
    {
        /// <summary>
        /// Emitted when the grabbable entity wants to cancel the grab.
        /// </summary>
        [Signal]
        public delegate void GrabCancelledEventHandler();
        [Signal]
        public delegate void GrabStartedEventHandler();
        [Signal]
        public delegate void GrabbingEventHandler(double delta);
        [Signal]
        public delegate void GrabEndedEventHandler();

        public bool CanGrab { get; set; }
        public Node2D Grabber { get; private set; }
        public Node2D AsNode2D => GetParent<Node2D>();

        [ExportCategory("Settings")]
        [Export]
        public Vector2 GrabOffset { get; private set; }

        public void OnGrabEnd()
        {
            EmitSignal(nameof(GrabEnded));
            Grabber = null;
        }

        public void OnGrabbing(double delta)
        {
            EmitSignal(nameof(Grabbing), delta);
        }

        public void OnGrabStart(Node2D grabber)
        {
            Grabber = grabber;
            EmitSignal(nameof(GrabStarted));
        }

        public void CancelGrab()
        {
            EmitSignal(nameof(GrabCancelled));
        }
    }
}