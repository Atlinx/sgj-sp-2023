using Godot;

namespace Game
{
	public partial class LimitRigidBodySpeed : Node 
    {
        [ExportCategory("Settings")]
        [Export]
        private float maxLinearSpeed;
        [Export]
        private float maxAngularSpeed;

        [ExportCategory("Dependencies")]
        [Export]
        private RigidBody2D rigidBody2D;

        public override void _PhysicsProcess(double delta) 
        {
            if (rigidBody2D.LinearVelocity.Length() > maxLinearSpeed)
                rigidBody2D.LinearVelocity = rigidBody2D.LinearVelocity.LimitLength(maxLinearSpeed);
            if (Mathf.Abs(rigidBody2D.AngularVelocity) > maxAngularSpeed)
                rigidBody2D.AngularVelocity = (rigidBody2D.AngularVelocity < 0 ? -1 : 1) * maxLinearSpeed;
        }
    }
}