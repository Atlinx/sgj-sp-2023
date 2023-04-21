using Godot;

namespace Game
{
    public partial class PlayerMouseInput : Node2D, IPlayerInput
    {
        [ExportCategory("Settings")]
        [Export]
        public bool Held { get; private set; }
        [Export]
        public Vector2 PlayerPosition
        {
            get
            {
                return GetViewport().GetMousePosition();
            }
            set
            {
                GetViewport().WarpMouse(value);
            }
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