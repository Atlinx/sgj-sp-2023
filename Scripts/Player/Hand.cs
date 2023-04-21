using Godot;

namespace Game
{
    // TODO: Remove hand and replace with player altogether
    public partial class Hand : Node2D
    {
        [Signal]
        public delegate void BodyEnteredHandRange(Node2D body);
        [Signal]
        public delegate void BodyExitedHandRange(Node2D body);

        public enum SpriteStateEnum
        {
            Open,
            Grab,
            Point,
        }

        private SpriteStateEnum spriteState;
        public SpriteStateEnum SpriteState
        {
            get => spriteState;
            set
            {
                if (spriteState == value) return;

                spriteState = value;
                var tween = GetTree().CreateTween();
                switch (spriteState)
                {
                    case SpriteStateEnum.Open:
                        baseSprite.Texture = handOpenBaseTexture;
                        colorSprite.Texture = handOpenTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1.1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case SpriteStateEnum.Grab:
                        baseSprite.Texture = handGrabBaseTexture;
                        colorSprite.Texture = handGrabTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 0.9f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", new Vector2(1, -1) * 32, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                    case SpriteStateEnum.Point:
                        baseSprite.Texture = handPointBaseTexture;
                        colorSprite.Texture = handPointTexture;
                        tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
                        tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                        break;
                }
            }
        }

        public bool CanMove { get; set; } = true;

        private PlayerStaticData staticPlayerData;
        private IPlayerInput input;

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
        private Area2D collider;
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

        public void Construct(PlayerStaticData staticPlayerData, IPlayerInput playerInput)
        {
            this.staticPlayerData = staticPlayerData;
            this.input = playerInput;
            colorSprite.SelfModulate = staticPlayerData.Color;
            shadowSprite.SelfModulate = new Color(staticPlayerData.Color, 0.5f);
        }

        public override void _Ready()
        {
            collider.AreaEntered += (area) => EmitSignal(nameof(BodyEnteredHandRange), area);
            collider.AreaExited += (area) => EmitSignal(nameof(BodyExitedHandRange), area);
            collider.BodyEntered += (body) => EmitSignal(nameof(BodyEnteredHandRange), body);
            collider.BodyExited += (body) => EmitSignal(nameof(BodyExitedHandRange), body);
        }

        public override void _Process(double delta)
        {
            if (CanMove)
                Position = input.PlayerPosition;
        }
    }
}