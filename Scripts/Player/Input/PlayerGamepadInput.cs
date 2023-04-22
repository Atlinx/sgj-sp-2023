using Godot;

namespace Game
{
    public partial class PlayerGamepadInput : PlayerVectorInput
    {
        [ExportCategory("Dependencies")]
        [Export]
        public PlayerStaticData PlayerStaticData { get; set; }

        public void Construct(PlayerStaticData playerStaticData)
        {
            PlayerStaticData = playerStaticData;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            int playerIndex = PlayerStaticData.Number - 1;
            Held = Input.IsJoyButtonPressed(playerIndex, JoyButton.X);
            movementDirection = new Vector2(Input.GetJoyAxis(playerIndex, JoyAxis.LeftX), Input.GetJoyAxis(playerIndex, JoyAxis.LeftY));
            HalfSpeed = !(Input.GetJoyAxis(playerIndex, JoyAxis.TriggerRight) > 0.2f);
            if (movementDirection.Length() < 0.2f)
                movementDirection = Vector2.Zero;
        }
    }
}