using Godot;

namespace Game
{
    public interface IEnable
    {
        bool Enabled { get; set; }
    }

    public partial class GeneralEnabler : Node, IEnable
    {
        [ExportCategory("Dependencies")]
        [Export]
        private NodePath[] enablers = new NodePath[0];
        [Export]
        private NodePath[] visibilityEnablers = new NodePath[0];

        private bool enabled;
        [Export]
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;

                if (!initialized) return;

                if (enablers != null)
                    foreach (var path in enablers)
                    {
                        var node = GetNode(path);
                        if (node is IEnable enabler)
                            enabler.Enabled = value;
                        else if (node is CollisionShape2D collisionShape)
                            collisionShape.Disabled = !value;
                        else
                            node.ProcessMode = value ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
                    }
                if (visibilityEnablers != null)
                    foreach (var path in visibilityEnablers)
                    {
                        var node = GetNode(path);
                        if (node is CanvasItem canvasItem)
                            canvasItem.Visible = value;
                    }
            }
        }

        private bool initialized = false;
        public override void _Ready()
        {
            initialized = true;
        }
    }
}