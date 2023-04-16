using Godot;
using System;

public partial class Controller : Node
{
	[Export]
	private Node2D _whisk;
	[Export]
	private Node2D _centerPoint;
	[Export]
	private Area2D _reticle;

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
		
		Label label = GetNode<Label>("Label");
		Vector2 direction = _whisk.Position - _centerPoint.Position;
		float angle = Mathf.Atan2(direction.Y, direction.X);

		label.Text = angle.ToString();
	}

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton emb)
        {
			if (emb.IsPressed() && emb.ButtonIndex == MouseButton.Left)
            {
				_held = true;
				GD.Print("held");
            }
			else
            {
				_held = false;
				GD.Print("unheld");
			}
        }
    }
}
