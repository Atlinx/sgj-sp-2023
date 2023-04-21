using Godot;

namespace Game
{
    public partial class GrabFollowGrabber : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private float grabbableLerpDuration = 0.1f;

        [ExportCategory("Dependencies")]
        [Export]
        private Grabbable grabbable;
        [Export]
        private Node2D target;

        private float grabbableLerpTime = 0f;

        public override void _Ready()
        {
            grabbable.GrabStarted += OnGrabStarted;
            grabbable.Grabbing += OnGrabbing;
        }

        private void OnGrabStarted()
        {
            grabbableLerpTime = 0;
        }

        private void OnGrabbing(double delta)
        {
            if (grabbableLerpTime < grabbableLerpDuration)
                grabbableLerpTime += (float)delta;
            if (grabbableLerpTime > grabbableLerpDuration)
                grabbableLerpTime = grabbableLerpDuration;
            target.Position = target.Position.Lerp(grabbable.Grabber.Position, grabbableLerpTime / grabbableLerpDuration);
            if (grabbableLerpTime / grabbableLerpDuration < 1)
                GD.Print($"Lerping: target pos: {target.Position} lerp time: {grabbableLerpTime} lerp duration: {grabbableLerpDuration} with {grabbableLerpTime / grabbableLerpDuration}");
        }
    }
}