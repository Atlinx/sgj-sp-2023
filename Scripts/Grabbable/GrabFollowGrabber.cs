using Godot;

namespace Game
{
    public partial class GrabFollowGrabber : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private float grabbableLerpDuration;

        [ExportCategory("Dependencies")]
        [Export]
        private Grabbable grabbable;
        [Export]
        private Node2D target;

        private float grabbableLerpTime;

        public override void _Ready()
        {
            grabbable.Grabbing += OnGrabbing;
        }

        private void OnGrabbing(double delta)
        {
            if (grabbableLerpTime < grabbableLerpDuration)
                grabbableLerpTime += (float)delta;
            else if (grabbableLerpTime > grabbableLerpDuration)
                grabbableLerpTime = grabbableLerpDuration;
            target.Position = grabbable.Grabber.Position.Lerp(target.Position, grabbableLerpTime / grabbableLerpDuration);
        }
    }
}