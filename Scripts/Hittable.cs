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

    public interface IHittable
    {
        public Node2D AsNode2D { get; }
        public void OnHit(Node2D hitter);
    }

    public partial class Hittable : Node, IHittable
    {
        [Signal]
        public delegate void HitEventHandler();

        public Node2D AsNode2D => GetParent<Node2D>();
        public Node2D Hitter { get; private set; }

        public void OnHit(Node2D hitter)
        {
            EmitSignal(nameof(Hit));
            Hitter = hitter;
        }
    }
}