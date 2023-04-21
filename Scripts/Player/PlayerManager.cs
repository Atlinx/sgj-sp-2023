using Godot;
using System.Collections.Generic;

namespace Game
{
    public partial class PlayerManager : Node
    {
        public List<Player> Players { get; private set; } = new List<Player>();

        [ExportCategory("Dependencies")]
        [Export]
        private PackedScene playerPrefab;
        [Export]
        private PackedScene gamepadInputPrefab;
        [Export]
        private PackedScene mouseInputPrefab;
        [Export]
        private PackedScene actionInputPrefab;
        [Export]
        private Node2D centerPoint;
        [Export]
        private Control playerUIContainer;
        [Export]
        private PackedScene playerUIPrefab;
        [Export]
        private Node2D playersContainer;

        public void StartGame(PlayerData[] players)
        {
            foreach (var child in playerUIContainer.GetChildren())
                child.QueueFree();
            foreach (var child in playersContainer.GetChildren())
                child.QueueFree();
            foreach (var player in players)
                AddPlayer(player);
        }

        public void AddPlayer(PlayerData playerData)
        {
            Node inputNode = null;
            switch (playerData.InputType)
            {
                case PlayerInputType.Gamepad:
                    inputNode = gamepadInputPrefab.Instantiate<PlayerGamepadInput>();
                    break;
                case PlayerInputType.Action:
                    inputNode = actionInputPrefab.Instantiate<PlayerActionInput>();
                    break;
                case PlayerInputType.Mouse:
                    inputNode = mouseInputPrefab.Instantiate<PlayerMouseInput>();
                    break;
            }

            Player player = playerPrefab.Instantiate<Player>();
            player.AddChild(inputNode);
            playersContainer.AddChild(player);
            player.Construct(playerData, centerPoint, inputNode as IPlayerInput);

            PlayerUI playerUI = playerUIPrefab.Instantiate<PlayerUI>();
            playerUIContainer.AddChild(playerUI);
            playerUI.Construct(player);

            Players.Add(player);
        }
    }
}