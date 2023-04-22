using Godot;

namespace Game
{
	/// <summary>
	/// Keeps track of the node's current average velocity and at any point, can set
	/// its rigidbody to that velocity, effectively chucking the rigidbody.
	/// </summary>
	public partial class Chuckable : Node
    {
        [ExportCategory("Settings")]
        [Export]
        private float speedSampleInterval = 0.1f;
        [Export]
        private int speedSamples = 10;
        [Export]
        private float speedMultiplier = 5f;

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
            //       The rigid body must be enabled when we se it's linear velocity
            rigidbody2D.Freeze = false;
            rigidbody2D.LinearVelocity = positionDelta * CalculateAverageSpeed() * speedMultiplier;
        }

        public override void _Ready()
        {
            speedDeltaBuffer = new float[speedSamples];
        }

        public override void _Process(double delta)
        {
            positionDelta = rigidbody2D.Position - prevPosition;
            speedBufferSampleTime += (float)delta;
            while (speedBufferSampleTime >= speedSampleInterval)
            {
                speedBufferSampleTime = speedBufferSampleTime - speedSampleInterval;
                speedDeltaBuffer[speedDeltaBufferIndex] = positionDelta.Length();
                speedDeltaBufferIndex++;
                if (speedDeltaBufferIndex >= speedDeltaBuffer.Length)
                    speedDeltaBufferIndex = 0;
            }
            prevPosition = rigidbody2D.Position;
        }
    }
}