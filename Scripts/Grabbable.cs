using Godot;

namespace Game
{
    public partial class Grabbable : Node, IGrabbable
    {
        [Signal]
        public delegate void GrabStartedEventHandler();
        [Signal]
        public delegate void GrabbingEventHandler(double delta);
        [Signal]
        public delegate void GrabEndedEventHandler();

        public Node2D Grabber { get; private set; }
        public Node2D AsNode2D => GetParent<Node2D>();

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
    }
}