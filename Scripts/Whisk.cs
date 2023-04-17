using Godot;

namespace Game
{
    public partial class Whisk : Node2D
    {
        [ExportCategory("Settings")]
        [Export]
        private StaticPlayerData _staticPlayerData;
        [ExportCategory("Dependencies")]
        [Export]
        private Sprite2D baseSprite;
        [Export]
        private Sprite2D colorSprite;
        [Export]
        private Sprite2D shadowSprite;
        [Export]
        public Area2D Collider { get; private set; }

        public void Construct(StaticPlayerData staticPlayerData)
        {
            this._staticPlayerData = staticPlayerData;
            colorSprite.SelfModulate = staticPlayerData.Color;
            shadowSprite.SelfModulate = new Color(staticPlayerData.Color, 0.5f);
        }
    }
}