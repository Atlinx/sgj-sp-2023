using Godot;

namespace Game
{
    public partial class DefaultIdleHandler : Node, IPlayerIdleHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Hand hand;

        public bool CanHandle() => true;

        public void OnIdled(double delta)
        {
            hand.SpriteState = Hand.SpriteStateEnum.Point;
        }
    }
}