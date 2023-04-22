using Godot;

namespace Game
{
    public partial class GrabbableFX : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private string grabStartedAnim = "default";
        [Export]
        private string grabEndedAnim = "default";

        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D animatedSprite;
        [Export]
        private Grabbable grabbable;

        public override void _Ready()
        {
            grabbable.GrabStarted += () => animatedSprite.Play(grabStartedAnim);
            grabbable.GrabEnded += () => animatedSprite.Play(grabEndedAnim);
        }
    }
}