using Godot;

namespace Game
{
    public enum PlayerInputType
    {
        Gamepad,
        Mouse,
        Action
    }

    public class PlayerData
    {
        public PlayerStaticData StaticData { get; set; }

        public PlayerInputType InputType { get; set; }
    }

    public partial class Player : Node
    {
        public PlayerData PlayerData { get; set; }

        [ExportCategory("Settings")]
        [Export]
        public float AverageAngularVelocity { get; private set; }

        [ExportCategory("Dependencies")]
        [Export]
        private Whisk whisk;
        [Export]
        private Hand hand;

        private IPlayerInput input;
        private Node2D centerPoint;

        public void Construct(PlayerData playerData, Node2D centerPoint, IPlayerInput input)
        {
            PlayerData = playerData;
            this.centerPoint = centerPoint;
            this.input = input;
            whisk.Construct(playerData.StaticData);
            hand.Construct(playerData.StaticData);
        }

        private bool prevHeld = false;
        private float whiskLerpTime = 0;
        private float whiskLerpDuration = 0.1f;

        public override void _Process(double delta)
        {
            hand.Position = input.PlayerPosition;

            // Hover state
            if (hand.State != Hand.StateEnum.Grab)
            {
                if (hand.Collider.OverlapsArea(whisk.Collider))
                    hand.State = Hand.StateEnum.Open;
                else
                    hand.State = Hand.StateEnum.Point;
            }


            if (input.Held != prevHeld)
            {
                prevHeld = input.Held;

                // Grab state
                if (input.Held && hand.Collider.OverlapsArea(whisk.Collider))
                {
                    hand.State = Hand.StateEnum.Grab;
                    whiskLerpTime = 0;
                }
                else
                {
                    hand.State = Hand.StateEnum.Point;
                }
            }

            if (hand.State == Hand.StateEnum.Grab)
            {
                if (whiskLerpTime < whiskLerpDuration)
                    whiskLerpTime += (float)delta;
                else if (whiskLerpTime > whiskLerpDuration)
                    whiskLerpTime = whiskLerpDuration;
                whisk.Position = whisk.Position.Lerp(input.PlayerPosition, whiskLerpTime / whiskLerpDuration);
            }

            Vector2 direction = whisk.Position - centerPoint.Position;
            float angle = Mathf.Atan2(direction.Y, direction.X);
            AverageAngularVelocity = MovingAverage(angle, delta);
        }

        private float lastAngle;
        private float[] angularVelocityDeltas = new float[DataPoints];
        private int angularVelocityCurrIndex = 0;
        private const int DataPoints = 30;

        private float MovingAverage(float newAngle, double delta)
        {
            float angleDelta = newAngle - lastAngle;

            // Pi to -pi
            // Delta should still be positive
            if (lastAngle > Mathf.Pi / 2 && newAngle < -Mathf.Pi / 2)
            {
                angleDelta = newAngle + Mathf.Tau - lastAngle;
            }
            // -Pi to pi
            // Delta should still be negative
            else if (lastAngle < -Mathf.Pi / 2 && newAngle > Mathf.Pi / 2)
            {
                angleDelta = newAngle - Mathf.Tau - lastAngle;
            }

            lastAngle = newAngle;

            float angleVelocity = angleDelta / (float)delta;

            angularVelocityDeltas[angularVelocityCurrIndex++] = angleVelocity;
            if (angularVelocityCurrIndex >= DataPoints)
                angularVelocityCurrIndex = 0;

            float sum = 0;
            foreach (float i in angularVelocityDeltas)
            {
                sum += i;
            }

            return sum / DataPoints;
        }
    }
}