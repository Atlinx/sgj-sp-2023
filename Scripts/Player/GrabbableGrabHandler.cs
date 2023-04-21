using Godot;
using System.Collections.Generic;

namespace Game
{
    public class GrabbableGrabHandler : Node, IGrabStartHandler, IGrabbingHandler, IGrabEndHandler
    {
        [ExportCategory("Settings")]
        [Export]
        private float grabbableLerpDuration = 0.1f;

        [ExportCategory("Dependencies")]
        [Export]
        private Hand hand;
        [Export]
        private Player player;

        /// <summary>
        /// Grabbables that are in the hand's range
        /// </summary>
        public HashSet<IGrabbable> CanGrabGrabbables { get; private set; } = new HashSet<IGrabbable>();
        public IGrabbable Grabbed { get; private set; }

        private float grabbableLerpTime = 0;

        private void OnAnythingEntered(Node2D body)
        {
            if (body.TryGetComponent(out IGrabbable grabbable) && !CanGrabGrabbables.Contains(grabbable))
            {
                CanGrabGrabbables.Add(grabbable);
            }
        }

        private void OnAnythingExited(Node2D body)
        {
            if (body.TryGetComponent(out IGrabbable grabbable) && !CanGrabGrabbables.Contains(grabbable))
            {
                CanGrabGrabbables.Add(grabbable);
            }
        }

        public bool CanHandle()
        {
            return CanGrabGrabbables.Count > 0;
        }

        public void OnGrabStart()
        {
            // Should only be called when CanGrabGrabbables.Count > 0
            float shortestDist = 0;
            IGrabbable shortestDistGrabbable = null;
            foreach (var grabbable in CanGrabGrabbables)
            {
                float currDist = grabbable.AsNode2D.GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
                if (shortestDistGrabbable == null)
                {
                    // Initialize to first grabbable if we don't have a grabbable yet
                    shortestDist = currDist;
                    shortestDistGrabbable = grabbable;
                }
            }

            Grabbed = shortestDistGrabbable;
            hand.SpriteState = Hand.SpriteStateEnum.Grab;
        }

        public void OnGrabbing(double delta)
        {
            // Hover state
            if (hand.SpriteState != Hand.SpriteStateEnum.Grab)
            {
                if (CanGrabGrabbables.Count > 0)
                    hand.SpriteState = Hand.SpriteStateEnum.Open;
                else
                    hand.SpriteState = Hand.SpriteStateEnum.Point;
            }

            if (grabbableLerpTime < grabbableLerpDuration)
                grabbableLerpTime += (float)delta;
            else if (grabbableLerpTime > grabbableLerpDuration)
                grabbableLerpTime = grabbableLerpDuration;
            Grabbed.AsNode2D.Position = Grabbed.AsNode2D.Position.Lerp(hand.Position, grabbableLerpTime / grabbableLerpDuration);
        }

        public void OnGrabEnd()
        {
            hand.SpriteState = Hand.SpriteStateEnum.Point;
        }
    }
}