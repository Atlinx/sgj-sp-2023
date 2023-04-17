using Godot;

namespace Game
{
    public partial class PlayerUI : Control
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Player _player;
        [Export]
        private Speedometer _speedometer;

        public void Construct(Player player)
        {
            this._player = player;
            _speedometer.SetFill(1, player.PlayerData.StaticData.Color);
        }

        public override void _Process(double delta)
        {
            _speedometer.SetFill(Mathf.Clamp(_player.AverageAngularVelocity / 30f, 0, 1));
        }
    }
}