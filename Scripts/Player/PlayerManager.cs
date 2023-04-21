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
        private PackedScene whiskPrefab;
        [Export]
        private PackedScene gamepadInputPrefab;
        [Export]
        private PackedScene mouseInputPrefab;
        [Export]
        private PackedScene mouseDragInputPrefab;
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
            {
                AddPlayer(player);
                AddWhisk(centerPoint);
            }
        }

        public void AddPlayer(PlayerData playerData)
        {
            Node inputNode = null;
            switch (playerData.InputType)
            {
                case PlayerInputType.Gamepad:
                    var gamepadInput = gamepadInputPrefab.Instantiate<PlayerGamepadInput>();
                    gamepadInput.Construct(playerData.StaticData);
                    inputNode = gamepadInput;
                    break;
                case PlayerInputType.Action:
                    var playerActionInput = actionInputPrefab.Instantiate<PlayerActionInput>();
                    inputNode = playerActionInput;
                    break;
                case PlayerInputType.Mouse:
                    inputNode = mouseInputPrefab.Instantiate<PlayerMouseInput>();
                    break;
                case PlayerInputType.MouseDrag:
                    inputNode = mouseDragInputPrefab.Instantiate<PlayerMouseDragInput>();
                    break;
            }

            Player player = playerPrefab.Instantiate<Player>();
            player.AddChild(inputNode);

            var playerInput = inputNode as IPlayerInput;
            playersContainer.AddChild(player);
            player.Construct(playerData, playerInput);

            PlayerUI playerUI = playerUIPrefab.Instantiate<PlayerUI>();
            playerUIContainer.AddChild(playerUI);
            playerUI.Construct(player);

            Players.Add(player);
        }

        public void AddWhisk(Node2D centerPoint)
        {
            Whisk whisk = whiskPrefab.Instantiate<Whisk>();
            whisk.Construct(centerPoint);
            AddChild(whisk);
        }
    }
}