using Godot;

namespace Game
{
    public partial class GameManager : Node
    {
        public int Score { get; set; }
        public double Time { get; private set; }
        public RandomNumberGenerator RNG { get; set; }

        [ExportCategory("Settings")]
        [Export]
        public double GameDuration { get; set; }
        [Export]
        public ulong Seed { get; set; }

        [ExportCategory("Dependencies")]
        [Export]
        private PlayerManager playerManager;
        [Export]
        private ObstacleManager obstacleManager;

        public void StartGame(PlayerData[] players)
        {
            RNG = new RandomNumberGenerator();
            RNG.Seed = Seed;
            playerManager.StartGame(players);
            obstacleManager.StartGame();
        }

        public override void _Process(double delta)
        {
            Time += delta;
            if (Time > GameDuration)
            {
                EndGame();
            }
        }

        public void EndGame()
        {
            // TODO: Implement quitting
            GD.Print("Game finished!");
            obstacleManager.EndGame();
        }
    }
}