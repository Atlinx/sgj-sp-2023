using Godot;
using System.Collections.Generic;

namespace Game
{
    public partial class GrabbableGrabHandler : Node, IPlayerGrabStartHandler, IPlayerGrabbingHandler, IPlayerGrabEndHandler, IPlayerIdleHandler, IPlayerClickHandler
    {
        [ExportCategory("Settings")]
        [Export]
        private float grabbableLerpDuration = 0.1f;

        [ExportCategory("Dependencies")]
        [Export]
        private Hand hand;

        /// <summary>
        /// Grabbables that are in the hand's range
        /// </summary>
        public HashSet<IGrabbable> CanGrabGrabbables { get; private set; } = new HashSet<IGrabbable>();
        public IGrabbable Grabbed { get; private set; }
        public bool IsGrabbing => Grabbed != null;

        private float grabbableLerpTime = 0;

        public override void _Ready()
        {
            hand.BodyEnteredHandRange += OnBodyEntered;
            hand.BodyExitedHandRange += OnBodyExited;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body.TryGetComponent(out IGrabbable grabbable) && !CanGrabGrabbables.Contains(grabbable))
                CanGrabGrabbables.Add(grabbable);
        }

        private void OnBodyExited(Node2D body)
        {
            if (body.TryGetComponent(out IGrabbable grabbable) && CanGrabGrabbables.Contains(grabbable))
                CanGrabGrabbables.Remove(grabbable);
        }

        public bool CanHandle()
        {
            return CanGrabGrabbables.Count > 0;
        }

        public void OnGrabStarted()
        {
            // Should only be called when CanGrabGrabbables.Count > 0
            float shortestDist = 0;
            IGrabbable shortestDistGrabbable = null;
            foreach (var grabbable in CanGrabGrabbables)
            {
                float currDist = grabbable.AsNode2D.GlobalPosition.DistanceSquaredTo(hand.GlobalPosition);
                if (shortestDistGrabbable == null)
                {
                    // Initialize to first grabbable if we don't have a grabbable yet
                    shortestDist = currDist;
                    shortestDistGrabbable = grabbable;
                }
            }

            Grabbed = shortestDistGrabbable;
            Grabbed.OnGrabStart(hand);
            hand.SpriteState = Hand.SpriteStateEnum.Grab;
        }

        public void OnGrabbing(double delta)
        {
            Grabbed?.OnGrabbing(delta);
        }

        public void OnGrabEnded()
        {
            hand.SpriteState = Hand.SpriteStateEnum.Point;
            Grabbed?.OnGrabEnd();
            Grabbed = null;
        }

        public void OnIdled(double delta)
        {
            if (!IsGrabbing)
                hand.SpriteState = Hand.SpriteStateEnum.Open;
        }

        public void OnClicked()
        {
            hand.SpriteState = Hand.SpriteStateEnum.Open;
        }
    }
}