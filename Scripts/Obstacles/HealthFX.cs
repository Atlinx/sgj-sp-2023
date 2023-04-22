using Godot;

namespace Game
{
    public partial class HealthFX : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Health health;
        [Export]
        private GeneralFX opt_healedFx;
        [Export]
        private GeneralFX opt_damagedFx;
        [Export]
        private GeneralFX opt_deathFx;

        public override void _Ready()
        {
            health.Healed += () => opt_healedFx?.Play();
            health.Damaged += () => opt_damagedFx?.Play();
            health.Death += () => opt_deathFx?.Play();
        }
    }
}