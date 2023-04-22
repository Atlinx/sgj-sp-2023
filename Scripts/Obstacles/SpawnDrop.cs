using Godot;

namespace Game
{
    public partial class SpawnDrop : Node
    {
        [Signal]
        public delegate void SpawnDropLandingEventHandler();

        [ExportCategory("Settings")]
        [Export]
        private float dropSpeed;

        [ExportCategory("Dependencies")]
        [Export]
        private PathFollow2D dropFollow;
        [Export]
        private NodePath opt_enabler;
        public IEnable Enabler => GetNode<IEnable>(opt_enabler);

        private bool falling;

        public void BeginDrop()
        {
            falling = true;
            dropFollow.Loop = false;
            dropFollow.ProgressRatio = 0;
        }

        public override void _Process(double delta)
        {
            if (!falling) return;

            dropFollow.ProgressRatio += (float)delta * dropSpeed;
            if (dropFollow.ProgressRatio >= 1)
            {
                if (Enabler != null)
                    Enabler.Enabled = true;
                Land();
            }
        }

        public void Land()
        {
            if (Enabler != null)
                Enabler.Enabled = true;
            falling = false;
            EmitSignal(SignalName.SpawnDropLanding);
        }
    }
}