using Godot;

namespace Game
{
    public interface IClickable
    {
        public Node2D AsNode2D { get; }
        public void OnClicked(Node2D grabber);
    }

    public partial class Clickable : Node, IClickable
    {
        [Signal]
        public delegate void ClickedEventHandler();

        public Node2D AsNode2D => GetParent<Node2D>();
        public Node2D Clicker { get; private set; }

        public void OnClicked(Node2D clicker)
        {
            EmitSignal(nameof(Clicked));
            Clicker = clicker;
        }
    }
}