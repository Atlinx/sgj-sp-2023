using Godot;
using System;

namespace Game
{
    public partial class PlayerMainMenuUI : Node
    {
        [Signal]
        public delegate void RemovedEventHandler();

        public PlayerData PlayerData { get; set; } = new PlayerData();

        [ExportCategory("Dependencies")]
        [Export]
        private Button removePlayerButton;
        [Export]
        private OptionButton inputOptionButton;
        [Export]
        private TextureRect handOutlineRect;
        [Export]
        private Label playerLabel;

        public override void _Ready()
        {
            removePlayerButton.Pressed += () => EmitSignal(nameof(Removed));
            foreach (PlayerInputType input in Enum.GetValues(typeof(PlayerInputType)))
                inputOptionButton.AddItem(Enum.GetName(input), (int)input);
            inputOptionButton.ItemSelected += OnInputItemSelected;
        }

        public void Construct(PlayerData playerData)
        {
            PlayerData = playerData;
            Construct(playerData.StaticData);
        }

        public void Construct(PlayerStaticData staticData)
        {
            PlayerData.StaticData = staticData;
            handOutlineRect.SelfModulate = staticData.Color;
            playerLabel.Text = $"PLAYER {staticData.Number}";
        }

        private void OnInputItemSelected(long index)
        {
            PlayerData.InputType = (PlayerInputType)index;
        }
    }
}