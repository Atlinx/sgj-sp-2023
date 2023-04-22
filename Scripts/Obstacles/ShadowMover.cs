using Godot;

namespace Game
{
	public partial class ShadowMover : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Node entity;
        [Export]
        private Node2D shadow;
        [Export]
        private Grabbable grabbable;
        [Export]
        private Vector2 shadowOffsetOnEntity;

        public override void _Ready()
        {
            grabbable.GrabStarted += () =>
            {
                shadow.Reparent(entity);
                entity.MoveChild(shadow, 0);
                shadow.Position = shadowOffsetOnEntity;
            };
        }
    }
}