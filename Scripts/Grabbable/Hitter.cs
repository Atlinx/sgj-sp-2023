using Godot;

namespace Game
{
    public partial class Hitter : Node
    {
        [ExportCategory("Dependnecies")]
        [Export]
        private Area2D collider;

        public override void _Ready()
        {
            collider.BodyEntered += OnBodyEntered;
            collider.AreaEntered += OnBodyEntered;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body.TryGetComponent(out IClickable clickable))
                clickable.OnClicked(GetParent<Node2D>());
        }
    }
}