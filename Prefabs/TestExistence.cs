using Godot;
using System;

public partial class TestExistence : RigidBody2D
{
    public override void _Ready()
    {
        GD.Print("Hello world!");
    }

    public override void _Process(double delta)
    {
        GD.Print(Name + " at " + GlobalPosition);
        GD.Print(" Parent is " + GetParent().Name);
    }

    public override void _ExitTree()
    {
        GD.Print("Goodbye, world!");
    }
}
