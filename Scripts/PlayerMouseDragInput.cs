using Godot;

namespace Game
{
    public partial class PlayerMouseDragInput : Node2D, IPlayerInput
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
        private float movementSpeed;
        private Vector2 prevPosition = Vector2.Zero;

        public override void _Ready()
        {
            prevPosition = GetGlobalMousePosition();
        }

        public override void _Process(double delta)
        {
            var movementVector = GetGlobalMousePosition() - prevPosition;
            var maxMovementMagnitude = movementSpeed * (float)delta; // How much we can move in the current frame
            movementVector = movementVector.LimitLength(maxMovementMagnitude);
            PlayerPosition += movementVector;

            prevPosition = GetGlobalMousePosition();
        }
    }
}