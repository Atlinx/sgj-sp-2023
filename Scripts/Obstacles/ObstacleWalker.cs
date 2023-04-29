using Game;
using Godot;
using System;

public partial class ObstacleWalker : ObstacleSpawner
{
    [Export]
    private GameManager gameManager;
    [Export]
    private float minHeight;
    [Export]
    private float maxHeight;
    [Export]
    private float sidewaysDisplacement;

    public override bool CanHandle(Node obstacle)
    {
        return obstacle.HasComponent<SpawnWalk>();
    }

    public override void Spawn(Node obstacle)
    {
        Node2D obstacle2D = (Node2D)obstacle;

        int direction = 1;

        /*int direction = gameManager.RNG.Randf() >= 0.5 ? -1 : 1;
        if (direction < 0)
        {
            obstacle2D.Scale = new Vector2(-1 * obstacle2D.Scale.X, obstacle2D.Scale.Y);
        }*/

        float height = gameManager.RNG.Randf() * (maxHeight - minHeight) + minHeight;

        Vector2 coordinates = new Vector2(sidewaysDisplacement * direction, height);

        obstacle2D.Position = coordinates;
        obstacle2D.GetComponent<SpawnWalk>().BeginWalk();
    }
}
