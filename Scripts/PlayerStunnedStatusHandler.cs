using Godot;

namespace Game
{
    public partial class PlayerStunnedStatusHandler : HashSetStatusHandler, IStatusHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private StatusHolder statusHolder;
        [Export]
        private Player player;

        public override bool CanHandle(IStatus status)
        {
            return status is StunnedStatus;
        }

        public override void AddStatus(IStatus status)
        {
            base.AddStatus(status);
            if (!player.Disabled)
            {
                player.Disabled = true;
                var darkGrey = new Color(0.5f, 0.5f, 0.5f);
                player.Modulate = darkGrey;
            }
        }

        public override void RemoveStatus(IStatus status)
        {
            base.RemoveStatus(status);
            if (HandledStatuses.Count > 0)
            {
                player.Disabled = false;
                player.Modulate = Colors.White;
            }
        }

        public override void _Process(double delta)
        {
            foreach (var status in HandledStatuses)
                if (status is StunnedStatus stunnedStatus)
                {
                    stunnedStatus.Duration -= (float)delta;
                    if (stunnedStatus.Duration <= 0)
                        statusHolder.RemoveStatus(stunnedStatus);
                }
        }
    }
}