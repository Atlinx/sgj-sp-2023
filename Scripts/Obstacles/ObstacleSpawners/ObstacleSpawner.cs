using Godot;

namespace Game
{
    public abstract partial class ObstacleSpawner : Node
    {
        public abstract bool CanHandle(Node obstacle);
        public abstract void Spawn(Node obstacle);
    }
}