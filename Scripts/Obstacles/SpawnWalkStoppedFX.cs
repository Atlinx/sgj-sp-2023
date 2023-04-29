using Godot;

namespace Game
{
    public partial class SpawnWalkStoppedFX : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private string defaultAnim = "default";
        [Export]
        private string stoppedAnim = "stopped";

        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D animatedSprite;
        [Export]
        private SpawnWalk spawnWalk;

        public override void _Ready()
        {
            spawnWalk.SpawnWalkStopped += () => animatedSprite.Play(stoppedAnim);
            animatedSprite.Play(defaultAnim);
        }
    }
}