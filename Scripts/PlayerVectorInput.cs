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
        [ExportCategory("Dependencies")]
        [Export]
        public PlayerStaticData PlayerStaticData { get; set; }
        [Export]
        protected float movementSpeed;
        protected Vector2 movementDirection;

        public override void _Process(double delta)
        {
            PlayerPosition += movementDirection * movementSpeed * (float)delta;
        }
    }
}