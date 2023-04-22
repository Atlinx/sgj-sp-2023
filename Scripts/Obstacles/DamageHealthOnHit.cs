using Godot;

namespace Game
{
	/// <summary>
	/// Paired up with DamageHealthOnHitDetectors
	/// </summary>
	public partial class DamageHealthOnHit : Node 
    {
        [ExportCategory("Settings")]
        [Export]
        public int Damage { get; set; }
    }
}