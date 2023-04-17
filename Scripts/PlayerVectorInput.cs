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
        public StaticPlayerData StaticPlayerData { get; set; }
        [Export]
        private float _movementSpeed;
        protected Vector2 _movementDirection;

        public override void _Process(double delta)
        {
            PlayerPosition += _movementDirection * (float)delta;
        }
    }
}