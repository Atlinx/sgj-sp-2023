using Godot;
using MonoCustomResourceRegistry;

namespace Game
{
    [RegisteredType(nameof(PlayerStaticData), "", nameof(Resource))]
    public partial class PlayerStaticData : Resource
    {
        [Export]
        public int Number { get; set; }
        [Export]
        public Color Color { get; set; }
    }
}