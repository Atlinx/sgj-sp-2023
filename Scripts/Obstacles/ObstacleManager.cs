using Godot;
using GDC = Godot.Collections;

namespace Game
{
    public partial class ObstacleManager : Node
    {
        [Export]
        public PackedScene[] obstaclePrefabs;
        [Export]
        public GDC.Array<Node2D> Obstacles { get; private set; } = new GDC.Array<Node2D>();

        [ExportCategory("Settings")]
        [Export]
        public float DifficultyRampInterval { get; set; }
        [Export]
        public float SpawnInterval { get; set; }
        [Export]
        public int SpawnAmount { get; set; }
        [Export]
        public int MaxSpawnAmount { get; set; }
        [Export]
        public int MinSpawnInterval { get; set; }
        [Export]
        public int MaxObstacles { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private GameManager gameManager;
        [Export]
        private Node2D obstacleContainer;
        [Export]
        private ObstacleSpawner[] obstacleSpawners;

        private double spawnTime = 0;
        private double difficultyRampTime = 0;

        private RandomNumberGenerator rng;

        // TODO: Add specific location obstacle spawning
        public void StartGame()
        {
            if (obstaclePrefabs.Length == 0)
            {
                SetProcess(false);
                GD.PushWarning($"{nameof(ObstacleManager)}: No obstacle prefabs, so ObstacleManger has been stopped.");
                return;
            }
            foreach (var child in obstacleContainer.GetChildren())
                child.QueueFree();
            SetProcess(true);
            spawnTime = 0;
        }

        public void EndGame()
        {
            SetProcess(false);
        }

        public override void _Process(double delta)
        {
            spawnTime += delta;
            if (spawnTime >= SpawnInterval)
            {
                spawnTime -= SpawnInterval;
                SpawnObstacles();
            }

            difficultyRampTime += delta;
            if (difficultyRampTime >= DifficultyRampInterval)
            {
                difficultyRampTime -= DifficultyRampInterval;
                RampDifficulty();
            }
        }

        public void SpawnObstacles()
        {
            for (int i = 0; i < SpawnAmount; i++)
            {
                var prefab = RandomObstaclePrefab();
                var obstacle = prefab.Instantiate();
                obstacleContainer.AddChild(obstacle);

                bool foundSpawner = false;
                foreach (var spawner in obstacleSpawners)
                    if (spawner.CanHandle(obstacle))
                    {
                        spawner.Spawn(obstacle);
                        foundSpawner = true;
                        break;
                    }
                if (!foundSpawner)
                    GD.PrintErr($"{nameof(ObstacleManager)}: Could not find spawner for obstacle: {obstacle}.");
            }
        }

        public PackedScene RandomObstaclePrefab()
        {
            return obstaclePrefabs[gameManager.RNG.RandiRange(0, obstaclePrefabs.Length - 1)];
        }

        public void RampDifficulty()
        {
            if (SpawnAmount < MaxSpawnAmount)
                SpawnAmount++;
            if (SpawnInterval > MinSpawnInterval)
                SpawnInterval -= 0.1f;
        }
    }
}