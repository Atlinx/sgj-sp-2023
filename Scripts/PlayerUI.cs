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

        public void Construct(Player player)
        {
            this.player = player;
            speedometer.SetFill(1, player.PlayerData.StaticData.Color);
        }

        public override void _Process(double delta)
        {
            speedometer.SetFill(Mathf.Clamp(player.AverageAngularVelocity / 30f, 0, 1));
        }
    }
}