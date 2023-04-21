using Godot;

namespace Game
{
    public partial class PlayerMouseInput : Node2D, IPlayerInput
    {
        [ExportCategory("Settings")]
        [Export]
        public bool Held { get; private set; }
        private Vector2 playerPosition;
        [Export]
        public Vector2 PlayerPosition
        {
            get => playerPosition;
            set
            {
                if (playerPosition != value)
                    GetViewport().WarpMouse(value);
            }
        }

        public override void _Process(double delta)
        {
            playerPosition = GetViewport().GetMousePosition();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
                Held = mouseButtonEvent.Pressed;
        }

        public override void _EnterTree()
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
        }

        public override void _ExitTree()
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}