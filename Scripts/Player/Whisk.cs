using Godot;

namespace Game
{
    public partial class Whisk : Area2D
    {
        [ExportCategory("Settings")]
        [Export]
        private PlayerStaticData _staticPlayerData;
        [ExportCategory("Dependencies")]
        [Export]
        private Sprite2D baseSprite;
        [Export]
        private Sprite2D colorSprite;
        [Export]
        private Sprite2D shadowSprite;

        public void Construct(PlayerStaticData staticPlayerData)
        {
            this._staticPlayerData = staticPlayerData;
            colorSprite.SelfModulate = staticPlayerData.Color;
            shadowSprite.SelfModulate = new Color(staticPlayerData.Color, 0.5f);
        }
    }
}