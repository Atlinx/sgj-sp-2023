using Godot;

namespace Game
{
    public partial class PlayerUI : Control
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Player player;
        [Export]
        private Speedometer speedometer;
        [Export]
        private Label playerNameLabel;

        public void Construct(Player player)
        {
            this.player = player;
            speedometer.SetFill(1, player.PlayerData.StaticData.Color);
            playerNameLabel.Text = "Player " + player.PlayerData.StaticData.Number;
        }

        public override void _Process(double delta)
        {
            speedometer.SetFill(Mathf.Clamp(player.AverageAngularVelocity / 30f, 0, 1));
        }
    }
}