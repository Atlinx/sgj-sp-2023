using Godot;
using System;

public partial class ShootObstacle : Node
{
    [Export]
    private PackedScene obstacleToShoot;
    [Export]
    private Node2D originPoint;
    [Export]
    private Vector2 velocity;
    [Export]
    private Timer shootTimer;

    public override void _Ready()
    {
        shootTimer.Timeout += Shoot;
    }

    private void Shoot()
    {
        try
        {
            RigidBody2D rb = (RigidBody2D)obstacleToShoot.Instantiate();

            rb.Position = originPoint.Position;
            rb.LinearVelocity = velocity;

            GD.Print("Shot " + rb);
            GD.Print(rb.LinearVelocity);
        }
        catch (InvalidCastException)
        {
            GD.Print("Can only shoot rigidbodies ATM");
        }
    }
}
