using Godot;

namespace Game
{
    public partial class DefaultIdleHandler : Node, IPlayerIdleHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Player player;

        public bool CanHandle() => true;

        public void OnIdled(double delta)
        {
            player.SpriteState = Player.SpriteStateEnum.Point;
        }
    }
}