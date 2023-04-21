using Godot;

namespace Game
{
    public partial class HandDisableMovementStatusHandler : HashSetStatusHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Hand hand;

        public override bool CanHandle(IStatus status)
        {
            return status is DisableMovementStatus;
        }

        public override void AddStatus(IStatus status)
        {
            base.AddStatus(status);
            hand.CanMove = false;
        }

        public override void RemoveStatus(IStatus status)
        {
            base.RemoveStatus(status);
            if (HandledStatuses.Count == 0)
                hand.CanMove = true;
        }
    }
}