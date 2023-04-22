using Godot;

namespace Game
{
    public partial class WhiskHittable : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private int score;

        [ExportCategory("Dependencies")]
        [Export]
        private Hittable hittable;
        [Export]
        private GameManager gameManager;

        public override void _Ready()
        {
            hittable.Hit += OnHit;
        }

        private void OnHit()
        {
            gameManager.Score -= score;
        }
    }
}