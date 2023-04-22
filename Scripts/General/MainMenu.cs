using Godot;
using System.Linq;

namespace Game
{
    public partial class MainMenu : Node
    {
        [ExportCategory("Dependencies")]
        [Export]
        private PlayerStaticData[] playerStaticDatas;
        [Export]
        private Control playerMainMenuUIContainer;
        [Export]
        private Button playButton;
        [Export]
        private Button quitButton;
        [Export]
        private Button addPlayerButton;
        [Export]
        private PackedScene playerMainMenuUIPrefab;

        private int playerCount = 0;

        public override void _Ready()
        {
            addPlayerButton.Pressed += AddNewPlayer;
            quitButton.Pressed += OnQuitButtonPressed;
            playButton.Pressed += OnPlayButtonPressed;
            playButton.Disabled = true;

            foreach (Node child in playerMainMenuUIContainer.GetChildren())
                child.QueueFree();
        }

        private void OnPlayButtonPressed()
        {
            var playerDataManager = this.GetAutoload<PlayerDataManager>();
            playerDataManager.StartGame(GetPlayerDataArray());
        }

        private void OnQuitButtonPressed()
        {
            GetTree().Quit();
        }

        private void AddNewPlayer()
        {
            var playerMainMenuUI = playerMainMenuUIPrefab.Instantiate<PlayerMainMenuUI>();
            var nextPlayerStaticData = playerStaticDatas[playerCount++];
            if (playerCount >= playerStaticDatas.Length)
                addPlayerButton.Visible = false;

            playerMainMenuUIContainer.AddChild(playerMainMenuUI);
            playerMainMenuUI.Construct(nextPlayerStaticData);
            playerMainMenuUI.Name = nextPlayerStaticData.Number.ToString();
            playerMainMenuUI.Removed += () => OnPlayerRemoved(playerMainMenuUI);

            playButton.Disabled = false;
        }

        public void LoadPlayerDatas(PlayerData[] playerDatas)
        {
            foreach (Node child in playerMainMenuUIContainer.GetChildren())
                child.QueueFree();
            foreach (var playerData in playerDatas)
            {
                AddNewPlayer();
                var newPlayerUI = playerMainMenuUIContainer.GetChild(playerMainMenuUIContainer.GetChildCount() - 1) as PlayerMainMenuUI;
                newPlayerUI.Construct(playerData);
            }
        }

        private void OnPlayerRemoved(PlayerMainMenuUI removedUI)
        {
            // If we remove a player from the middle of the player list, we want
            // to adjust the player static datas so we still get a consecutive list of
            // players
            playerCount--;
            removedUI.QueueFree();
            int idx = 0;
            foreach (Node child in playerMainMenuUIContainer.GetChildren())
            {
                var currUI = child as PlayerMainMenuUI;
                if (currUI != removedUI)
                    currUI.Construct(playerStaticDatas[idx++]);
            }
            if (playerCount < playerStaticDatas.Length)
                addPlayerButton.Visible = true;
            if (playerCount == 0)
                playButton.Disabled = true;
        }

        private PlayerData[] GetPlayerDataArray()
        {
            return playerMainMenuUIContainer.GetChildren().Cast<PlayerMainMenuUI>().Select(x => x.PlayerData).ToArray();
        }
    }
}