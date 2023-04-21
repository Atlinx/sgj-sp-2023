using Godot;

namespace Game
{
    public partial class Grabbable : Node2D, IGrabbable
    {
        [Signal]
        public delegate void GrabStarted();
        [Signal]
        public delegate void GrabEnded();

        public Node2D AsNode2D => this;

        [ExportCategory("Settings")]
        [Export]
        public bool IsFixed { get; set; }

        public void OnGrabEnd()
        {
            EmitSignal(nameof(GrabEnded));
        }

        public void OnGrabStart()
        {
            EmitSignal(nameof(GrabStarted));
        }
    }
}