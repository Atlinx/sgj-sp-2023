using Godot;

namespace Game
{
    /// <summary>
    /// Used for quick tests with the player.
    /// </summary>
    public partial class ManualPlayerConstruct : Node
    {
        [ExportCategory("Settings")]
        [Export]
        public StaticPlayerData StaticData { get; set; }

        [Export]
        public PlayerInputType InputType { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private Player _player;
        [Export]
        private PlayerUI _playerUI;
        [Export]
        private Node2D _centerPoint;

        public override void _Ready()
        {
            CallDeferred(nameof(DeferredReady));
        }

        private void DeferredReady()
        {
            // Initialize the player after the player has readied itself
            _player.Construct(new PlayerData()
            {
                StaticData = StaticData,
                InputType = InputType
            }, _centerPoint);
            _playerUI.Construct(_player);
        }
    }
}