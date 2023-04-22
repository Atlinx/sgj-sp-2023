using Godot;

namespace Game
{
    /// <summary>
    /// FX that detaches itself from the parent when played, usually used for death effects where the effects linger after the parent has been freed.
    /// </summary>
    public partial class DeathFX : GeneralFX
    {
        [ExportCategory("Settings")]
        [Export]
        public bool Unparent { get; set; } = true;
        [Export]
        public float Lifetime { get; set; } = 1f;

        private float time = 0;

        public override void _Ready()
        {
            SetProcess(false);
        }

        public override void Play()
        {
            base.Play();
            SetProcess(true);
            ServiceLocator.Global.GetService<World>().AddNode(this, World.Category.FX);
        }

        public override void _Process(double delta)
        {
            time += (float)delta;
            if (time > Lifetime)
                QueueFree();
        }
    }
}