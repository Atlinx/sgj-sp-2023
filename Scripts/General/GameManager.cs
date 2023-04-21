﻿using Godot;

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

        [Export] public int WhiskTickScore { get; set; } = 100;
        
        [Export] public float PlayerMaxAngularVelocity { get; set; } = 30;

        [ExportCategory("Dependencies")]
        [Export]
        private PlayerManager playerManager;
        [Export]
        private ObstacleManager obstacleManager;
        [Export] 
        public Bowl bowl;
        [Export] 
        public Label scoreLabel;

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
            var scoreMultiplier = (int)bowl.TotalAverageAngularVelocity;
            foreach(var player in playerManager.Players)
            {
                if (player.GetWhisk() is not null)
                {
                    Score += WhiskTickScore * scoreMultiplier;
                }
            }

            scoreLabel.Text = "Score: " + Score;
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