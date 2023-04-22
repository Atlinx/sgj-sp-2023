using Godot;

namespace Game
{
    public abstract partial class PlayerVectorInput : Node, IPlayerInput
    {
        [ExportCategory("Settings")]
        [Export]
        public bool Held { get; protected set; }
        [Export]
        public Vector2 PlayerPosition { get; set; }
        [Export]
        protected float movementSpeed;
        protected Vector2 movementDirection;
        [Export]
        public bool HalfSpeed { get; protected set; }

        public override void _Process(double delta)
        {
            var moveDelta = movementDirection * movementSpeed * (float)delta;
            if (HalfSpeed)
                moveDelta *= 0.5f;
            PlayerPosition += moveDelta;
        }
    }
}