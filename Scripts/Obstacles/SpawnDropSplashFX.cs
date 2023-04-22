using Godot;

namespace Game
{
	public partial class SpawnDropSplashFX : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private string defaultAnim = "default";
        [Export]
        private string submergedAnim = "submerged";

        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D animatedSprite;
        [Export]
        private SpawnDrop spawnDrop;

        public override void _Ready()
        {
            spawnDrop.SpawnDropLanding += () => animatedSprite.Play(submergedAnim);
            animatedSprite.Play(defaultAnim);
        }
    }
}