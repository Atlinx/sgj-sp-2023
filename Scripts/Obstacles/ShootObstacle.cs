using Godot;
using System;

namespace Game {

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
        private World world;

        public override void _Ready()
        {
            shootTimer.Timeout += Shoot;
            world = ServiceLocator.Global.GetService<World>();
        }

        private void Shoot()
        {
            try
            {
                RigidBody2D rb = (RigidBody2D)obstacleToShoot.Instantiate();

                rb.GlobalPosition = originPoint.GlobalPosition;
                rb.LinearVelocity = velocity;
                world.AddNode(rb);
            }
            catch (InvalidCastException)
            {
                GD.Print("Can only shoot rigidbodies ATM");
            }
        }
    }
}
