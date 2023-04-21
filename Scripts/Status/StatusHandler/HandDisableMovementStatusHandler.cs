using Godot;

namespace Game
{
    public partial class HandDisableMovementStatusHandler : HashSetStatusHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Player player;

        public override bool CanHandle(IStatus status)
        {
            return status is DisableMovementStatus;
        }

        public override void AddStatus(IStatus status)
        {
            base.AddStatus(status);
            player.CanMove = false;
        }

        public override void RemoveStatus(IStatus status)
        {
            base.RemoveStatus(status);
            if (HandledStatuses.Count == 0)
                player.CanMove = true;
        }
    }
}