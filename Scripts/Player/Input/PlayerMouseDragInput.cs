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
        [Export]
        private float movementSpeed;

        public override void _Ready()
        {
            PlayerPosition = GetGlobalMousePosition();
        }

        public override void _Process(double delta)
        {
            var movementVector = GetGlobalMousePosition() - PlayerPosition;
            var maxMovementMagnitude = movementSpeed * (float)delta; // How much we can move in the current frame
            movementVector = movementVector.LimitLength(maxMovementMagnitude);
            PlayerPosition += movementVector;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
                Held = mouseButtonEvent.Pressed;
        }
    }
}