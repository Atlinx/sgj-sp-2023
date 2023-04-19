using Godot;

namespace Game
{
    public interface IPlayerInput
    {
        public bool Held { get; }
        public Vector2 PlayerPosition { get; set; }
        public PlayerStaticData PlayerStaticData { get; set; }
    }
}