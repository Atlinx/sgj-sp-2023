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
        private ObstacleManager obstacleSpanwer;

        public void StartGame(PlayerData[] players)
        {
            RNG = new RandomNumberGenerator();
            RNG.Seed = Seed;
            foreach (var player in players)
                playerManager.AddPlayer(player);
            obstacleSpanwer.StartGame();
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
            obstacleSpanwer.EndGame();
        }
    }
}