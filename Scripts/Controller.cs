using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class Controller : Node
{
	[Export]
	private Node2D _whisk;
	[Export]
	private Node2D _centerPoint;
	[Export]
	private Area2D _reticle;
	[Export]
	private Speedometer _speedometer;

	private bool _held = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		_reticle.Position = GetViewport().GetMousePosition();

		if (_held && _reticle.OverlapsArea(_whisk.GetNode<Area2D>("Area2D")))
        {
			_whisk.Position = GetViewport().GetMousePosition();
        }
		
		Vector2 direction = _whisk.Position - _centerPoint.Position;
		float angle = Mathf.Atan2(direction.Y, direction.X);

		float average = MovingAverage(angle, delta);

		_speedometer.SetFill(Mathf.Clamp(average / 30f, 0, 1));
	}

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton emb)
        {
			if (emb.IsPressed() && emb.ButtonIndex == MouseButton.Left)
            {
				_held = true;
            }
			else
            {
				_held = false;
			}
        }
    }

    private float _lastAngle;
    private List<float> _angularVelocityDeltas = new List<float>();
	private const int DATAPOINTS = 30;

    private float MovingAverage(float newAngle, double delta)
	{
        float angleDelta = newAngle - _lastAngle;

		// Pi to -pi
		// Delta should still be positive
        if (_lastAngle > Mathf.Pi / 2 && newAngle < -Mathf.Pi / 2)
		{
			angleDelta = newAngle + Mathf.Tau - _lastAngle;
		}
		// -Pi to pi
		// Delta should still be negative
        else if (_lastAngle < -Mathf.Pi / 2 && newAngle > Mathf.Pi / 2)
		{
			angleDelta = newAngle - Mathf.Tau - _lastAngle;
        }

        _lastAngle = newAngle;

        float angleVelocity = angleDelta / (float)delta;

        _angularVelocityDeltas.Add(angleVelocity);
		if (_angularVelocityDeltas.Count > DATAPOINTS) {
            _angularVelocityDeltas.RemoveAt(0);
        }

		float sum = 0;
		foreach (float i in _angularVelocityDeltas)
		{
			sum += i;
		}

		return sum / DATAPOINTS;
	}
}
