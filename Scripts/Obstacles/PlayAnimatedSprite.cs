using Godot;

namespace Game
{
    public partial class PlayAnimatedSprite : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private string animation;

        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D animatedSprite;

        public void Play()
        {
            animatedSprite.Play(animation);
        }
    }
}