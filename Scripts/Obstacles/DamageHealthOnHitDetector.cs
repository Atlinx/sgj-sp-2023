using Godot;

namespace Game
{
    /// <summary>
    /// Paired up with DamageHealthOnHit.
    /// </summary>
    public partial class DamageHealthOnHitDetector : Node 
    {
        [ExportCategory("Dependencies")]
        [Export]
        private RigidBody2D physicsBody2D;
        [Export]
        private Health health;

        public override void _Ready() 
        {
            physicsBody2D.BodyEntered += (body) => 
            {
                if (body.TryGetComponent(out DamageHealthOnHit damageHealthOnHit)) 
                {
                    GD.Print("Took" + damageHealthOnHit.Damage + " damage!");
                    health.Damage(damageHealthOnHit.Damage);
                }
            }; 
        }
    }
}