using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public partial class Bowl : Node2D
	{
		public const float MaxBowlVelocity = 30f;

		[ExportCategory("Settings")]
		[Export]
		private float resistance;
		[Export]
		private int velocitySamples = 30;

		[ExportCategory("Dependencies")]
		[Export]
		public Node2D CenterPoint { get; private set; }

		[Export]
		public Area2D Area { get; private set; }

		[Export]
		private AnimatedSprite2D fillSprite;
		[Export]
		private AnimatedSprite2D centerPointSprite;

		private float[] linearVelocityDeltaBuffer;
		private float[] angularVelocityDeltaBuffer;
		private int velocityCurrIndex = 0;
		private List<Whisk> whisks = new List<Whisk>();

		public float TotalAverageAngularVelocity { get; private set; }
		public float TotalAverageLinearVelocity { get; private set; }

		public override void _Ready()
		{
			fillSprite.Play();
			centerPointSprite.Play();
			angularVelocityDeltaBuffer = new float[velocitySamples];
			linearVelocityDeltaBuffer = new float[velocitySamples];
		}

		public void RegisterWhisk(Whisk whisk)
		{
			whisks.Add(whisk);
		}

		private void UpdateAverageVelocity()
		{
			float angularSum = 0;
			foreach (Whisk whisk in whisks)
				angularSum += whisk.AverageAngularVelocity;

			linearVelocityDeltaBuffer[velocityCurrIndex] = whisks
				.Select(x => x.AverageLinearVelocity)
				.Average();
			angularVelocityDeltaBuffer[velocityCurrIndex++] = angularSum /= whisks.Count;
			if (velocityCurrIndex >= angularVelocityDeltaBuffer.Length)
				velocityCurrIndex = 0;

			float grandSum = 0;
			foreach (float i in angularVelocityDeltaBuffer)
				grandSum += i;

			TotalAverageAngularVelocity = grandSum / angularVelocityDeltaBuffer.Length;
			TotalAverageLinearVelocity = linearVelocityDeltaBuffer.Average();
		}

		public override void _Process(double delta)
		{
			UpdateAverageVelocity();
			//fillSprite.SpeedScale = TotalAverageAngularVelocity / resistance;
			fillSprite.SpeedScale = TotalAverageLinearVelocity / resistance;
		}
	}
}
