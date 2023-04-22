using Godot;

namespace Game
{
    public partial class Health : Node
    {
        [Signal]
        public delegate void DeathEventHandler();

        [ExportCategory("Settings")]
        [Export]
        private int Amount { get; set; }
        [Export]
        private int Max { get; set; }

        public void Damage(int amount)
        {
            if (Amount <= 0)
                return;
            Amount -= amount;
            if (Amount <= 0)
                EmitSignal(nameof(Death));
        }

        public void Heal(int amount)
        {
            if (Amount == Max)
                return;
            Amount += amount;
            if (Amount > Max)
                Amount = Max;
        }
    }
}