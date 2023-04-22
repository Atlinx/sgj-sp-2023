using Godot;

namespace Game
{
    public partial class GrabChuck : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Grabbable grabbale;
        [Export]
        private Chuckable chuckable;

        public override void _Ready()
        {
            grabbale.GrabEnded += OnGrabEnded;
        }

        private void OnGrabEnded()
        {
            chuckable.Chuck();
        }
    }
}