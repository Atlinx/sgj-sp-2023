using Godot;

namespace Game
{
    public partial class Bob : Node2D
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
        [Export]
        private Clickable clickable;
        [Export]
        private Area2D collider;

        [Export]
        private int maxHealth = 1;

        private int health = 0;

        public override void _Ready()
        {
            baseSprite.Play();
            spawnDrop.SpawnDropLanding += OnLand;
            clickable.Clicked += TakeDamage;
            health = maxHealth;
            collider.ProcessMode = ProcessModeEnum.Disabled;
        }

        private void OnLand()
        {
            collider.ProcessMode = ProcessModeEnum.Inherit;
            baseSprite.Animation = submergedAnimation;
            fallingShadow.Visible = false;
        }

        private void TakeDamage()
        {
            health -= 1;
            if (health == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            QueueFree();
        }
    }
}