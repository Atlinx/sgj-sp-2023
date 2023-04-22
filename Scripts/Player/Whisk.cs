using Godot;
using System.Linq;

namespace Game
{
    public partial class Whisk : Node2D
    {
        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D baseSprite;
        [Export]
        private Area2D tip;
        [Export]
        private Sprite2D whiskShadow;
        [Export]
        private Sprite2D tipShadow;
        [Export]
        private Hittable hittable;

        [ExportCategory("Settings")]
        [Export]
        public float AverageAngularVelocity { get; private set; }
        [Export]
        public float AverageLinearVelocity { get; private set; }
        [Export]
        private int velocitySamples = 30;
        [Export]
        private float velocitySampleInterval = 0.1f;
        [Export]
        private int crashPenalty;
        [Export]
        private int stunTime;

        public Player currentPlayer { get; set; }

        private float[] linearVelocityDeltaBuffer;
        private float[] angularVelocityDeltaBuffer;
        private int velocityCurrIndex = 0;
        private float velocitySampleTime = 0;
        private float prevAngle;
        private Bowl bowl;
        private GameManager manager;

        private const string SubmergedAnimStr = "whisk_sub";
        private const string DefaultAnimStr = "whisk";

        private StunnedStatus stunnedStatus;

        public void Construct(GameManager manager, Bowl bowl)
        {
            this.bowl = bowl;
            this.manager = manager;
            bowl.RegisterWhisk(this);
            angularVelocityDeltaBuffer = new float[velocitySamples];
            linearVelocityDeltaBuffer = new float[velocitySamples];
        }

        public override void _Ready()
        {
            baseSprite.Play();
            hittable.Hit += OnHit;
            stunnedStatus = new StunnedStatus(stunTime);
        }

        public override void _Process(double delta)
        {
            if (tip.OverlapsArea(bowl.Area))
            {
                baseSprite.Animation = SubmergedAnimStr;
                whiskShadow.Visible = false;
                tipShadow.Visible = true;
            }
            else
            {
                baseSprite.Animation = DefaultAnimStr;
                whiskShadow.Visible = true;
                tipShadow.Visible = false;
            }
            UpdateAverageAngularVelocity(delta);
        }

        private void UpdateAverageAngularVelocity(double delta)
        {
            velocitySampleTime += (float)delta;
            while (velocitySampleTime > velocitySampleInterval)
            {
                velocitySampleTime -= velocitySampleInterval;

                Vector2 direction = Position - bowl.CenterPoint.Position;
                float radius = direction.Length();
                float newAngle = Mathf.Atan2(direction.Y, direction.X);


                float angleDelta = newAngle - prevAngle;
                // Pi to -pi
                // Delta should still be positive
                if (prevAngle > Mathf.Pi / 2 && newAngle < -Mathf.Pi / 2)
                    angleDelta = newAngle + Mathf.Tau - prevAngle;
                // -Pi to pi
                // Delta should still be negative
                else if (prevAngle < -Mathf.Pi / 2 && newAngle > Mathf.Pi / 2)
                    angleDelta = newAngle - Mathf.Tau - prevAngle;

                prevAngle = newAngle;

                float angleVelocity;
                if (!tip.OverlapsArea(bowl.Area))
                {
                    angleVelocity = 0;
                }
                else
                {
                    angleVelocity = angleDelta / (float)delta;
                }

                angularVelocityDeltaBuffer[velocityCurrIndex] = angleVelocity;
                linearVelocityDeltaBuffer[velocityCurrIndex] = (angleVelocity * radius);
                velocityCurrIndex++;

                if (velocityCurrIndex >= angularVelocityDeltaBuffer.Length)
                    velocityCurrIndex = 0;

                float angularSum = 0;
                foreach (float i in angularVelocityDeltaBuffer)
                    angularSum += i;

                AverageAngularVelocity = angularSum / angularVelocityDeltaBuffer.Length;
                AverageLinearVelocity = linearVelocityDeltaBuffer.Sum() / linearVelocityDeltaBuffer.Length;
            }
        }

        private void OnHit()
        {
            if (currentPlayer != null)
            {
                currentPlayer.StatusHolder.AddStatus(stunnedStatus);
                manager.Score -= crashPenalty;
            }
        }
    }
}
