using Godot;

namespace Game
{
    /// <summary>
    /// Keeps track of the node's current average velocity and at any point, can set
    /// its rigidbody to that velocity, effectively chucking the rigidbody.
    /// </summary>
    public class Chuckable : Node2D
    {
        [ExportCategory("Settings")]
        [Export]
        private float speedSsampleInterval = 0.1f;
        [Export]
        private int speedSamples = 10;

        [ExportCategory("Dependencies")]
        [Export]
        private RigidBody2D rigidbody2D;

        private float[] speedDeltaBuffer;
        private int speedDeltaBufferIndex = 0;

        private float speedBufferSampleTime = 0;
        private Vector2 prevPosition;
        private Vector2 positionDelta;

        public float CalculateAverageSpeed()
        {
            float total = 0;
            for (int i = 0; i < speedDeltaBuffer.Length; i++)
            {
                total += speedDeltaBuffer[i];
            }
            return total / speedDeltaBuffer.Length;
        }

        public void Chuck()
        {
            // TODO: Is it possible to disable a rigidbody?
            rigidbody2D.LinearVelocity = positionDelta * CalculateAverageSpeed();
        }

        public override void _Ready()
        {
            speedDeltaBuffer = new float[speedSamples];
        }

        public override void _Process(double delta)
        {
            positionDelta = rigidbody2D.Position - prevPosition;
            speedBufferSampleTime += (float)delta;
            while (speedBufferSampleTime >= speedSsampleInterval)
            {
                speedBufferSampleTime = speedBufferSampleTime - speedSsampleInterval;
                speedDeltaBuffer[speedDeltaBufferIndex] = positionDelta.Length;
                speedDeltaBufferIndex++;
                if (speedDeltaBufferIndex >= speedDeltaBuffer.Length)
                    speedDeltaBufferIndex = 0;
            }
            prevPosition = rigidbody2D.Position;
        }
    }
}