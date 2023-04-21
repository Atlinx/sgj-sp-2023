using Game;
using Godot;
using System;

public partial class ObstacleDropper : ObstacleSpawner
{
    [Export]
    private Bowl bowl;
    [Export]
    private GameManager gameManager;

    public override bool CanHandle(Node obstacle)
    {
        return obstacle.HasComponent<SpawnDrop>();
    }

    public override void Spawn(Node obstacle)
    {
        Node2D obstacle2D = (Node2D)obstacle;

        float angle = gameManager.RNG.Randf() * Mathf.Tau;
        Vector2 displacement = new Vector2(gameManager.RNG.Randf() * bowl.Radius, 0);
        displacement = displacement.Rotated(angle);

        obstacle2D.Position = bowl.CenterPoint.Position + displacement;
        GD.Print("Displacement:" + displacement);
        GD.Print("Calculated position relative to bowl:" + (bowl.CenterPoint.Position + displacement));
        GD.Print("Actual position:" + obstacle2D.Position);
        obstacle2D.GetComponent<SpawnDrop>().BeginDrop();
    }
}
