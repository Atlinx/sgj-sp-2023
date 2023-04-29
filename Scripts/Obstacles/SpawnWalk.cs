using Godot;

namespace Game
{
    public partial class SpawnWalk : Node
    {
        [Signal]
        public delegate void SpawnWalkStoppedEventHandler();

        [ExportCategory("Settings")]
        [Export]
        private float walkSpeed;

        [ExportCategory("Dependencies")]
        [Export]
        private PathFollow2D walkFollow;
        [Export]
        private NodePath opt_enabler;
        public IEnable Enabler => GetNode<IEnable>(opt_enabler);

        private bool walking;

        public void BeginWalk()
        {
            walking = true;
            walkFollow.Loop = false;
            walkFollow.ProgressRatio = 0;

            if (Enabler != null)
                Enabler.Enabled = false;
        }

        public override void _Process(double delta)
        {
            if (!walking) return;

            walkFollow.ProgressRatio += (float)delta * walkSpeed;
            if (walkFollow.ProgressRatio >= 1)
            {
                if (Enabler != null)
                    Enabler.Enabled = true;
                Stopped();
            }
        }

        public void Stopped()
        {
            if (Enabler != null)
                Enabler.Enabled = true;
            walking = false;
            EmitSignal(SignalName.SpawnWalkStopped);
        }
    }
}
