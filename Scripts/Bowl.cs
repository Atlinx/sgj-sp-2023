using Godot;
using System;

public partial class Bowl : Node2D
{
    [ExportCategory("Settings")]
    [Export]
    private float resistance;

    [ExportCategory("Dependencies")]
    [Export]
    public Node2D CenterPoint { get; private set; }

    [Export]
    public Area2D Area { get; private set; }

    [Export]
    private AnimatedSprite2D Fill;
    [Export]
    private AnimatedSprite2D X;

    public override void _Ready()
    {
        Fill.Play();
        X.Play();
    }

    public void ChangeSpeed(float speed)
    {
        Fill.SpeedScale = speed / resistance;
    }
}
