using Godot;

namespace Game
{
    public partial class Whisk : Area2D
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Sprite2D baseSprite;

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
        private Node2D centerPoint;

        public void Construct(Node2D centerPoint)
        {
            this.centerPoint = centerPoint;
            angularVelocityDeltaBuffer = new float[angularVelocitySamples];
        }

        public override void _Process(double delta)
        {
            UpdateAverageAngularVelocity(delta);
        }

        private void UpdateAverageAngularVelocity(double delta)
        {
            angularVelocitySampleTime += (float)delta;
            while (angularVelocitySampleTime > angularVelocitySampleInterval)
            {
                angularVelocitySampleTime -= angularVelocitySampleInterval;

                Vector2 direction = Position - centerPoint.Position;
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

                float angleVelocity = angleDelta / (float)delta;

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