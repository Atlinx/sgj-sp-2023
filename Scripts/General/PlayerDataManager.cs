using Godot;

namespace Game
{
    /// <summary>
    /// Persistent class that lives through scene changes
    /// </summary>
    public partial class PlayerDataManager : Node
    {
        public PlayerData[] PlayerDatas { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private PackedScene gameplayScene;
        [Export]
        private PackedScene menuScene;

        public override void _Ready()
        {
            // Make this an autoload by making it a child of the root
            CallDeferred("reparent", GetTree().Root);
        }

        public void StartGame(PlayerData[] playerDatas = null)
        {
            if (playerDatas != null)
                PlayerDatas = playerDatas;
            var result = GetTree().ChangeSceneToPacked(gameplayScene);
            if (result != Error.Ok)
            {
                GD.PushError($"{nameof(PlayerDataManager)}: Change scene to gameplay failed.");
                return;
            }
            var gameManager = GetTree().CurrentScene.GetComponent<GameManager>();
            gameManager.GameFinished += OnGameFinished;
            gameManager.StartGame(PlayerDatas);
        }

        private void OnGameFinished()
        {
            var result = GetTree().ChangeSceneToPacked(menuScene);
            if (result != Error.Ok)
            {
                GD.PushError($"{nameof(PlayerDataManager)}: Change scene to menu failed.");
                return;
            }
            var mainMenu = GetTree().CurrentScene.GetComponent<MainMenu>();
            mainMenu.LoadPlayerDatas(PlayerDatas);
        }
    }
}