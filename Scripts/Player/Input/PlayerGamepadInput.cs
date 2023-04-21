using Godot;

namespace Game
{
    public partial class PlayerGamepadInput : PlayerVectorInput
    {
        public override void _Process(double delta)
        {
            base._Process(delta);
            int playerIndex = PlayerStaticData.Number - 1;
            Held = Input.IsJoyButtonPressed(playerIndex, JoyButton.X);
            movementDirection = new Vector2(Input.GetJoyAxis(playerIndex, JoyAxis.LeftX), Input.GetJoyAxis(playerIndex, JoyAxis.LeftY));
        }
    }
}