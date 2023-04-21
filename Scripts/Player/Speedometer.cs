using Godot;

namespace Game
{
    public partial class Speedometer : ColorRect
    {
        [Export]
        private ColorRect _fill;

        public void SetFill(float ratio, Color color)
        {
            _fill.Size = new Vector2(ratio * Size.X, Size.Y);
            _fill.Color = color;
        }

        public void SetFill(float ratio)
        {
            SetFill(ratio, _fill.Color);
        }
    }

}