using Godot;

namespace Game
{
    public partial class QueueFreeDeath : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Health health;
        [Export]
        private Node target;

        public override void _Ready()
        {
            health.Death += OnDeath;
        }

        public void OnDeath()
        {
            target.QueueFree();
        }
    }
}