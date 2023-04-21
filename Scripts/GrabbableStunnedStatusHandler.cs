using Godot;
using System.Linq;

namespace Game
{
    public class StunnedStatus : IStatus
    {
        public float Duration { get; set; }
    }

    public partial class GrabbableStunnedStatusHandler : HashSetStatusHandler, IStatusHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private StatusHolder statusHolder;
        [Export]
        private Grabbable grabbable;
        [Export]
        private NodePath[] canvasItemNodes;
        private CanvasItem[] canvasItems;

        public override void _Ready()
        {
            canvasItems = canvasItemNodes.Select(x => GetNode<CanvasItem>(x)).ToArray();
        }

        public override bool CanHandle(IStatus status)
        {
            return status is StunnedStatus;
        }

        public override void AddStatus(IStatus status)
        {
            base.AddStatus(status);
            grabbable.CancelGrab();
            grabbable.CanGrab = false;
            var darkGrey = new Color(0.5f, 0.5f, 0.5f);
            foreach (var item in canvasItems)
                item.Modulate = darkGrey;
        }

        public override void RemoveStatus(IStatus status)
        {
            base.RemoveStatus(status);
            if (HandledStatuses.Count > 0)
            {
                grabbable.CanGrab = true;
                foreach (var item in canvasItems)
                    item.Modulate = Colors.White;
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