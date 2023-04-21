using Godot;

namespace Game
{
    public partial class GrabKeepGrabberStill : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private float grabbableLerpDuration;

        [ExportCategory("Dependencies")]
        [Export]
        private Grabbable grabbable;
        [Export]
        private Node2D target;
        private DisableMovementStatus disableMovementStatus = new DisableMovementStatus();

        public override void _Ready()
        {
            grabbable.GrabStarted += OnGrabStarted;
            grabbable.GrabEnded += OnGrabEnded;
        }

        private void OnGrabStarted()
        {
            if (grabbable.Grabber.TryGetComponent(out StatusHolder statusHolder))
                statusHolder.AddStatus(disableMovementStatus);
        }

        private void OnGrabEnded()
        {
            if (grabbable.Grabber.TryGetComponent(out StatusHolder statusHolder))
                statusHolder.RemoveStatus(disableMovementStatus);
        }
    }
}