using Godot;

namespace Game
{
    public partial class ComponentRouter : Node
    {
        [ExportCategory("Settings")]
        [Export]
        public Node Source { get; private set; }
    }
}