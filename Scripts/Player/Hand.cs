using Godot;

namespace Game
{
	// TODO: Remove hand and replace with player altogether
	public partial class Hand : Node2D
	{
		[Signal]
		public delegate void BodyEnteredHandRangeEventHandler(Node2D body);
		[Signal]
		public delegate void BodyExitedHandRangeEventHandler(Node2D body);

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
						baseSprite.Animation = handOpenAnimation;
						colorSprite.Animation = handOpenAnimation;
						tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1.1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
						tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
						break;
					case SpriteStateEnum.Grab:
						baseSprite.Animation = handGrabAnimation;
						colorSprite.Animation = handGrabAnimation;
						tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
						tween.TweenProperty(spritesContainer, "position", GrabOffset, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
						break;
					case SpriteStateEnum.Point:
						baseSprite.Animation = handPointerAnimation;
						colorSprite.Animation = handPointerAnimation;
						tween.TweenProperty(spritesContainer, "scale", Vector2.One * 1f, 0.1f).SetTrans(Tween.TransitionType.Bounce);
						tween.TweenProperty(spritesContainer, "position", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
						break;
				}
			}
		}

		public Vector2 GrabOffset { get; set; }
		public bool CanMove { get; set; } = true;

		private PlayerStaticData staticPlayerData;
		private IPlayerInput input;

		[ExportCategory("Dependencies")]
		[Export]
		private Node2D spritesContainer;
		[Export]
		private AnimatedSprite2D baseSprite;
		[Export]
		private AnimatedSprite2D colorSprite;
		[Export]
		private Area2D collider;
		[Export]
		private string handOpenAnimation;
		[Export]
		private string handGrabAnimation;
		[Export]
		private string handPointerAnimation;

		public void Construct(PlayerStaticData staticPlayerData, IPlayerInput playerInput)
		{
			this.staticPlayerData = staticPlayerData;
			this.input = playerInput;
			colorSprite.SelfModulate = staticPlayerData.Color;
			colorSprite.Play();
			baseSprite.Play();
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
