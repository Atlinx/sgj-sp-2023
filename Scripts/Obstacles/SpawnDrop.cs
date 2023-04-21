using Godot;
using System;

public partial class SpawnDrop : Node
{
	[Signal]
	public delegate void SpawnDropLandingEventHandler();

	[Export]
	private float dropSpeed;
	[Export]
	private PathFollow2D dropFollow;

	private bool falling;

	public void BeginDrop()
    {
		falling = true;
		dropFollow.Loop = false;
		dropFollow.ProgressRatio = 0;
    }

    public override void _Process(double delta)
    {
		if (!falling) return;

		dropFollow.ProgressRatio += (float)delta * dropSpeed;
		if (dropFollow.ProgressRatio >= 1)
        {
			Land();
        }
    }

	public void Land()
    {
		falling = false;
		GD.Print("Called land");
		EmitSignal(SignalName.SpawnDropLanding);
    }
}
