using Godot;

namespace Game
{
    /// <summary>
    /// Used for quick tests with the player.
    /// </summary>
    public partial class ManualStart : Node
    {
        [ExportCategory("Settings")]
        [Export]
        public PlayerStaticData StaticData { get; set; }
        [Export]
        public PlayerInputType InputType { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private GameManager gameManager;

        public override void _Ready()
        {
            CallDeferred(nameof(DeferredReady));
        }

        private void DeferredReady()
        {
            gameManager.StartGame(new PlayerData[] { GetPlayerData() });
        }

        private PlayerData GetPlayerData() => new PlayerData()
        {
            InputType = InputType,
            StaticData = StaticData
        };
    }
}