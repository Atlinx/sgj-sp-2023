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
        public StaticPlayerData StaticData { get; set; }

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
        private Whisk _whisk;
        [Export]
        private Hand _hand;
        [Export]
        private PackedScene _gamepadInputPrefab;
        [Export]
        private PackedScene _mouseInputPrefab;
        [Export]
        private PackedScene _actionInputPrefab;

        private Node _inputNode;
        private IPlayerInput _input => (IPlayerInput)_inputNode;
        private Node2D _centerPoint;

        public void Construct(PlayerData playerData, Node2D centerPoint)
        {
            PlayerData = playerData;
            _centerPoint = centerPoint;
            switch (playerData.InputType)
            {
                case PlayerInputType.Gamepad:
                    _inputNode = _gamepadInputPrefab.Instantiate<PlayerGamepadInput>();
                    AddChild(_inputNode);
                    break;
                case PlayerInputType.Action:
                    _inputNode = _actionInputPrefab.Instantiate<PlayerActionInput>();
                    AddChild(_inputNode);
                    break;
                case PlayerInputType.Mouse:
                    _inputNode = _mouseInputPrefab.Instantiate<PlayerMouseInput>();
                    AddChild(_inputNode);
                    break;
            }
            _whisk.Construct(playerData.StaticData);
            _hand.Construct(playerData.StaticData);
        }

        private bool prevHeld = false;
        private float whiskLerpTime = 0;
        private float whiskLerpDuration = 0.1f;

        public override void _Process(double delta)
        {
            _hand.Position = _input.PlayerPosition;

            // Hover state
            if (_hand.State != Hand.StateEnum.Grab)
            {
                if (_hand.Collider.OverlapsArea(_whisk.Collider))
                    _hand.State = Hand.StateEnum.Open;
                else
                    _hand.State = Hand.StateEnum.Point;
            }

            // Grab state
            if (_input.Held)
            {
                if (_input.Held != prevHeld)
                {
                    prevHeld = _input.Held;

                    if (_hand.Collider.OverlapsArea(_whisk.Collider))
                    {
                        _hand.State = Hand.StateEnum.Grab;
                        whiskLerpTime = 0;
                    }
                }
            }
            else
            {
                if (_input.Held != prevHeld)
                {
                    prevHeld = _input.Held;
                    _hand.State = Hand.StateEnum.Open;
                }
            }

            if (_hand.State == Hand.StateEnum.Grab)
            {
                if (whiskLerpTime < whiskLerpDuration)
                    whiskLerpTime += (float)delta;
                else if (whiskLerpTime > whiskLerpDuration)
                    whiskLerpTime = whiskLerpDuration;
                _whisk.Position = _whisk.Position.Lerp(_input.PlayerPosition, whiskLerpTime / whiskLerpDuration);
            }

            Vector2 direction = _whisk.Position - _centerPoint.Position;
            float angle = Mathf.Atan2(direction.Y, direction.X);
            AverageAngularVelocity = MovingAverage(angle, delta);
        }

        private float _lastAngle;
        private float[] _angularVelocityDeltas = new float[DATA_POINTS];
        private int _angularVelocityCurrIndex = 0;
        private const int DATA_POINTS = 30;

        private float MovingAverage(float newAngle, double delta)
        {
            float angleDelta = newAngle - _lastAngle;

            // Pi to -pi
            // Delta should still be positive
            if (_lastAngle > Mathf.Pi / 2 && newAngle < -Mathf.Pi / 2)
            {
                angleDelta = newAngle + Mathf.Tau - _lastAngle;
            }
            // -Pi to pi
            // Delta should still be negative
            else if (_lastAngle < -Mathf.Pi / 2 && newAngle > Mathf.Pi / 2)
            {
                angleDelta = newAngle - Mathf.Tau - _lastAngle;
            }

            _lastAngle = newAngle;

            float angleVelocity = angleDelta / (float)delta;

            _angularVelocityDeltas[_angularVelocityCurrIndex++] = angleVelocity;
            if (_angularVelocityCurrIndex >= DATA_POINTS)
                _angularVelocityCurrIndex = 0;

            float sum = 0;
            foreach (float i in _angularVelocityDeltas)
            {
                sum += i;
            }

            return sum / DATA_POINTS;
        }
    }
}