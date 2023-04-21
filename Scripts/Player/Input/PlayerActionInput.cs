using Godot;

namespace Game
{
    public partial class PlayerActionInput : PlayerVectorInput
    {
        [ExportCategory("Dependencies")]
        [Export]
        public PlayerStaticData PlayerStaticData { get; set; }

        public void Construct(PlayerStaticData playerStaticData)
        {
            PlayerStaticData = playerStaticData;
        }

        private string PrefixInput(string input) => $"p{PlayerStaticData.Number}_{input}";

        public override void _Process(double delta)
        {
            base._Process(delta);
            int playerIndex = PlayerStaticData.Number - 1;
            Held = Input.IsActionPressed(PrefixInput("Grab"));
            movementDirection = Input.GetVector(PrefixInput("left"), PrefixInput("right"), PrefixInput("up"), PrefixInput("down"));
        }
    }
}