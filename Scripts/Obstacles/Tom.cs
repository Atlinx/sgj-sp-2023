using Godot;

namespace Game
{
    public partial class Tom : Node2D
    {
        [Export]
        private SpawnDrop spawnDrop;
        [Export]
        private AnimatedSprite2D baseSprite;
        [Export]
        private string defaultAnimation;
        [Export]
        private string submergedAnimation;
        [Export]
        private Sprite2D fallingShadow;

        public override void _Ready()
        {
            baseSprite.Play();
            spawnDrop.SpawnDropLanding += OnLand;
        }

        private void OnLand()
        {
            baseSprite.Animation = submergedAnimation;
            fallingShadow.Visible = false;
        }
    }
}