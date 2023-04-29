using Godot;
using System;

namespace Game
{
    /// <summary>
    /// Holds all nodes in the world
    /// </summary>
    public partial class World : Node2D, IService
    {
        public enum Category
        {
            Default,
            Player,
            Obstacles,
            FX,
        }

        [Export]
        public string[] Categories { get; private set; }

        public override void _Ready()
        {
            ServiceLocator.Global.AddService(this);

            foreach (string category in Enum.GetNames(typeof(Category)))
            {
                if (!HasNode(category))
                {
                    var node = new Node2D();
                    node.Name = category;
                    AddChild(node);
                }
            }
        }

        public void AddNode(Node node) => AddNode(node, Category.Default);
        public void AddNode(Node node, Category category)
        {
            var categoryNode = GetCategory(category);
            if (categoryNode != null)
            {
                if (node.GetParent() == null)
                {
                    AddChild(node);
                }
                else node.Reparent(categoryNode);
            } 
        }

        public void RemoveNode(Node node) => RemoveNode(node, Category.Default);
        public void RemoveNode(Node node, Category category)
        {
            var categoryNode = GetCategory(category);
            if (categoryNode != null)
                categoryNode.RemoveChild(node);
        }

        public Node2D GetCategory(Category category) => GetNode<Node2D>(Enum.GetName(category));
    }
}