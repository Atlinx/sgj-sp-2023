using Godot;

namespace Game
{
    public partial class ClickDamage : Node
    {
        [ExportCategory("Settings")]
        [Export]
        public int Damage { get; set; } = 1;

        [ExportCategory("Dependencies")]
        [Export]
        private Clickable clickable;
        [Export]
        private Health health;

        public override void _Ready()
        {
            clickable.Clicked += () => health.Damage(Damage);
        }
    }
}