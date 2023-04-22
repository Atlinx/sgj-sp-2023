using Godot;
using System.Collections.Generic;

namespace Game
{
    public partial class ClickableClickHandler : Node, IPlayerClickHandler
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Player player;

        /// <summary>
        /// Grabbables that are in the hand's range
        /// </summary>
        public HashSet<IClickable> CanClickClickables { get; private set; } = new HashSet<IClickable>();
        public override void _Ready()
        {
            player.BodyEnteredHandRange += OnBodyEntered;
            player.BodyExitedHandRange += OnBodyExited;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body.TryGetComponent(out IClickable clickable) && !CanClickClickables.Contains(clickable))
                CanClickClickables.Add(clickable);
        }

        private void OnBodyExited(Node2D body)
        {
            if (body.TryGetComponent(out IClickable clickable) && CanClickClickables.Contains(clickable))
                CanClickClickables.Remove(clickable);
        }

        public bool CanHandle()
        {
            return CanClickClickables.Count > 0;
        }

        public void OnClicked()
        {
            float shortestDist = 0;
            IClickable shortestDistClickable = null;
            foreach (var clickable in CanClickClickables)
            {
                float currDist = clickable.AsNode2D.GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
                if (shortestDistClickable == null)
                {
                    // Initialize to first grabbable if we don't have a grabbable yet
                    shortestDist = currDist;
                    shortestDistClickable = clickable;
                }

            }
            shortestDistClickable.OnClicked(GetParent<Node2D>());
        }
    }
}