using Godot;

namespace Game
{
    public partial class Hand : Node2D
    {
        public enum StateEnum
        {
            Open,
            Grab,
            Point,
        }

        private StateEnum state;
        public StateEnum State
        {
            get => state;
            set
            {
                if (state == value) return;

                state = value;
                var tween = GetTree().CreateTween();
                switch (state)
                {
                    case StateEnum.Open:
                        baseSprite.Texture = handOpenBaseTexture;
                        colorSprite.Texture = handOpenTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1.1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case StateEnum.Grab:
                        baseSprite.Texture = handGrabBaseTexture;
                        colorSprite.Texture = handGrabTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 0.9f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", new Vector2(1, -1) * 32, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case StateEnum.Point:
                        baseSprite.Texture = handPointBaseTexture;
                        colorSprite.Texture = handPointTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                }
            }
        }

        [Export]
        private PlayerStaticData _staticPlayerData;

        [ExportCategory("Dependencies")]
        [Export]
        private Node2D spritesContainer;
        [Export]
        private Sprite2D baseSprite;
        [Export]
        private Sprite2D colorSprite;
        [Export]
        private Sprite2D shadowSprite;
        [Export]
        public Area2D Collider { get; private set; }
        [Export]
        private Texture2D handOpenTexture;
        [Export]
        private Texture2D handOpenBaseTexture;
        [Export]
        private Texture2D handGrabTexture;
        [Export]
        private Texture2D handGrabBaseTexture;
        [Export]
        private Texture2D handPointTexture;
        [Export]
        private Texture2D handPointBaseTexture;

        public void Construct(PlayerStaticData staticPlayerData)
        {
            this._staticPlayerData = staticPlayerData;
            colorSprite.SelfModulate = staticPlayerData.Color;
            shadowSprite.SelfModulate = new Color(staticPlayerData.Color, 0.5f);
        }
    }
}