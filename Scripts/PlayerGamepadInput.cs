using Godot;

namespace Game
{
    public partial class PlayerGamepadInput : PlayerVectorInput
    {
        public override void _Process(double delta)
        {
            base._Process(delta);
            int playerIndex = StaticPlayerData.Number - 1;
            Held = Input.IsJoyButtonPressed(playerIndex, JoyButton.X);
            _movementDirection = new Vector2(Input.GetJoyAxis(playerIndex, JoyAxis.LeftX), Input.GetJoyAxis(playerIndex, JoyAxis.LeftY));
        }
    }
}