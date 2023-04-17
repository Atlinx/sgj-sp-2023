using Godot;
using MonoCustomResourceRegistry;

namespace Game
{
    [RegisteredType(nameof(StaticPlayerData), "", nameof(Resource))]
    public partial class StaticPlayerData : Resource
    {
        [Export]
        public int Number { get; set; }
        [Export]
        public Color Color { get; set; }
    }
}