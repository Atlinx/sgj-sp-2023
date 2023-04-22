using Godot;

namespace Game
{
    public partial class QueueFreeDeath : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        public Health health;
        [Export]
        public DeathFX opt_deathFX;

        public override void _Ready()
        {
            health.Death += OnDeath;
        }

        public void OnDeath()
        {
            opt_deathFX?.Play();
            QueueFree();
        }
    }
}