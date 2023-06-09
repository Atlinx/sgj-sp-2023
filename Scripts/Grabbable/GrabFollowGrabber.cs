﻿using Godot;

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
            target.GlobalPosition = target.GlobalPosition.Lerp(grabbable.Grabber.GlobalPosition, grabbableLerpTime / grabbableLerpDuration);
        }
    }
}