using Godot;

namespace Game
{
    public partial class Health : Node
    {
        [Signal]
        public delegate void DeathEventHandler();
        [Signal]
        public delegate void DamagedEventHandler();
        [Signal]
        public delegate void HealedEventHandler();

        [ExportCategory("Settings")]
        [Export]
        private int Amount { get; set; }
        [Export]
        private int Max { get; set; }
        private bool Dead { get; set; } = false;

        public void Damage(int amount)
        {
            if (Amount <= 0 || Dead)
                return;
            Amount -= amount;
            EmitSignal(nameof(Damaged));
            if (Amount <= 0)
                EmitSignal(nameof(Death));
        }

        public void Heal(int amount)
        {
            if (Amount == Max || Dead)
                return;
            EmitSignal(nameof(Healed));
            Amount += amount;
            if (Amount > Max)
                Amount = Max;
        }
    }
}