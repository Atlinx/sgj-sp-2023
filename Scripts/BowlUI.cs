using Godot;

namespace Game
{
    public partial class BowlUI : Control
    {
        [ExportCategory("Dependencies")]
        [Export]
        private Bowl bowl;
        [Export]
        private Speedometer speedometer;

        public override void _Process(double delta)
        {
            speedometer.SetFill(Mathf.Clamp(bowl.TotalAverageLinearVelocity / Bowl.MaxBowlVelocity, 0, 1));
        }
    }
}