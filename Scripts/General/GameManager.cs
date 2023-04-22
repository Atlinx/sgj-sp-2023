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

        [Export] public int WhiskTickScore { get; set; } = 10;

        [ExportCategory("Dependencies")]
        [Export]
        private PlayerManager playerManager;
        [Export]
        private ObstacleManager obstacleManager;
        [Export] 
        public Bowl bowl;
        [Export] 
        public Label scoreLabel;

        private int lastTick = 0;
        private int scoreMultiplier = 0;
        private bool gameStarted = false;

        public void StartGame(PlayerData[] players)
        {
            RNG = new RandomNumberGenerator();
            RNG.Seed = Seed;
            playerManager.StartGame(players);
            obstacleManager.StartGame();
            gameStarted = true;
            Time = 0;
            lastTick = 0;
            foreach (var player in playerManager.Players)
            {
                player.CanMove = true;
            }
        }

        public override void _Process(double delta)
        {
            if (gameStarted)
            {
                Time += delta;
                scoreMultiplier = Mathf.Clamp((int)bowl.TotalAverageLinearVelocity / 540, 1, 10);
                if ((int)Time > lastTick)
                {
                    ScoreTick();
                    lastTick = (int)Time;
                }
                
                if (Time > GameDuration)
                {
                    EndGame();
                }
            }
        }

        public void AddPointsWithMultiplier(int points)
        {
            Score += points * scoreMultiplier;
        }
        
        public void RemovePointsNoMultiplier(int points)
        {
            Score -= points;
        }

        private void ScoreTick()
        {
            foreach(var player in playerManager.Players)
            {
                if (player.GetWhisk() is not null)
                {
                    Score += WhiskTickScore * scoreMultiplier;
                }
            }

            scoreLabel.Text = "Score: " + Score;
        }

        public void EndGame()
        {
            // TODO: Implement quitting
            GD.Print("Game finished!");
            foreach (var player in playerManager.Players)
            {
                player.CanMove = false;
            }
            gameStarted = false;
            obstacleManager.EndGame();
        }
    }
}