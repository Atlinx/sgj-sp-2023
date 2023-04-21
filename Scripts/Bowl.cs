using Godot;
using System.Collections.Generic;

namespace Game
{
    public partial class Bowl : Node2D
    {
        public const float MaxBowlVelocity = 30f;

        [ExportCategory("Settings")]
        [Export]
        private float resistance;
        [Export]
        private int angularVelocitySamples = 30;

        [ExportCategory("Dependencies")]
        [Export]
        public Node2D CenterPoint { get; private set; }

        [Export]
        public Area2D Area { get; private set; }
        [Export]
        private CollisionShape2D Collider;

        [Export]
        private AnimatedSprite2D fillSprite;
        [Export]
        private AnimatedSprite2D centerPointSprite;

        private float[] angularVelocityDeltaBuffer;
        private int angularVelocityCurrIndex = 0;
        private List<Whisk> whisks = new List<Whisk>();

        public float TotalAverageAngularVelocity { get; private set; }

        public float Radius { get { if (Collider.Shape is CircleShape2D circle) return circle.Radius; else return 0f; } }

        public override void _Ready()
        {
            fillSprite.Play();
            centerPointSprite.Play();
            angularVelocityDeltaBuffer = new float[angularVelocitySamples];
        }

        public void RegisterWhisk(Whisk whisk)
        {
            whisks.Add(whisk);
        }

        public void UpdateAverageAngularVelocity()
        {
            float sum = 0;
            foreach (Whisk whisk in whisks)
                sum += whisk.AverageAngularVelocity;

            angularVelocityDeltaBuffer[angularVelocityCurrIndex++] = sum /= whisks.Count;
            if (angularVelocityCurrIndex >= angularVelocityDeltaBuffer.Length)
                angularVelocityCurrIndex = 0;

            float grandSum = 0;
            foreach (float i in angularVelocityDeltaBuffer)
                grandSum += i;

            TotalAverageAngularVelocity = grandSum / angularVelocityDeltaBuffer.Length;
        }

        public override void _Process(double delta)
        {
            UpdateAverageAngularVelocity();
            fillSprite.SpeedScale = TotalAverageAngularVelocity / resistance;
        }
    }
}