using Godot;

namespace Game
{
    public partial class Whisk : Area2D
    {
        [ExportCategory("Dependencies")]
        [Export]
        private AnimatedSprite2D baseSprite;

        [Export]
        public float AverageAngularVelocity { get; private set; }
        [Export]
        private int angularVelocitySamples = 30;
        [Export]
        private float angularVelocitySampleInterval = 0.1f;

        private float[] angularVelocityDeltaBuffer;
        private int angularVelocityCurrIndex = 0;
        private float angularVelocitySampleTime = 0;
        private float prevAngle;
        private Bowl bowl;

        private const string SUBMERGED = "whisk_sub";
        private const string DEFAULT = "whisk";

        public void Construct(Bowl bowl)
        {
            this.bowl = bowl;
            angularVelocityDeltaBuffer = new float[angularVelocitySamples];
        }

        public override void _Ready()
        {
            baseSprite.Play();
        }

        public override void _Process(double delta)
        {
            if (OverlapsArea(bowl.Area))
            {
                baseSprite.Animation = SUBMERGED;
            }
            else
            {
                baseSprite.Animation = DEFAULT;
            }
            UpdateAverageAngularVelocity(delta);
        }

        private void UpdateAverageAngularVelocity(double delta)
        {
            angularVelocitySampleTime += (float)delta;
            while (angularVelocitySampleTime > angularVelocitySampleInterval)
            {
                angularVelocitySampleTime -= angularVelocitySampleInterval;

                Vector2 direction = Position - bowl.CenterPoint.Position;
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
                if (!OverlapsArea(bowl.Area))
                {
                    angleVelocity = 0;
                }
                else
                {
                    angleVelocity = angleDelta / (float)delta;
                }

                angularVelocityDeltaBuffer[angularVelocityCurrIndex++] = angleVelocity;
                if (angularVelocityCurrIndex >= angularVelocityDeltaBuffer.Length)
                    angularVelocityCurrIndex = 0;

                float sum = 0;
                foreach (float i in angularVelocityDeltaBuffer)
                    sum += i;

                AverageAngularVelocity = sum / angularVelocityDeltaBuffer.Length;
            }
        }
    }
}