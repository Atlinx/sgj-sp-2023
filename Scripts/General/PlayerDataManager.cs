using Godot;

namespace Game
{
    /// <summary>
    /// Persistent class that lives through scene changes
    /// </summary>
    public partial class PlayerDataManager : Node, IService
    {
        public PlayerData[] PlayerDatas { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private SceneManager sceneManager;

        public async void StartGame(PlayerData[] playerDatas = null)
        {
            if (playerDatas != null)
                PlayerDatas = playerDatas;
            await sceneManager.TransitionTo(SceneManager.Scene.Gameplay);
            var gameManager = ServiceLocator.Global.GetService<GameManager>();
            gameManager.GameFinished += OnGameFinished;
            gameManager.StartGame(PlayerDatas);
        }

        private async void OnGameFinished()
        {
            await sceneManager.TransitionTo(SceneManager.Scene.MainMenu);
            var mainMenu = ServiceLocator.Global.GetService<MainMenu>();
            mainMenu.LoadPlayerDatas(PlayerDatas);
        }
    }
}