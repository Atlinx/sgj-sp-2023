using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public partial class CompoundStatus : ICompoundStatus
    {
        public IStatus[] Statuses { get; set; }

        public CompoundStatus(IStatus[] statuses)
        {
            Statuses = statuses;
        }
    }

    public interface ICompoundStatus
    {
        IStatus[] Statuses { get; }
    }

    public interface IStatus
    {

    }

    public interface IStatusHandler
    {
        bool CanHandle(IStatus status);
        void AddStatus(IStatus status);
        void RemoveStatus(IStatus status);
    }

    public partial class StatusHolder : Node
    {
        public HashSet<IStatus> Statuses { get; private set; } = new HashSet<IStatus>();
        public IStatusHandler[] Handlers { get; private set; }

        [ExportCategory("Dependencies")]
        [Export]
        private NodePath[] handlerNodes = new NodePath[0];

        public override void _Ready()
        {
            Handlers = handlerNodes.Select(x => GetNode<IStatusHandler>(x)).ToArray();
        }

        public void AddStatus(IStatus status)
        {
            Statuses.Add(status);
            InternalAddStatus(status);
        }

        public void RemoveStatus(IStatus status)
        {
            Statuses.Remove(status);
            InternalRemoveStatus(status);
        }

        private void InternalAddStatus(IStatus status)
        {
            if (status is ICompoundStatus compoundStatus)
                foreach (var subStatus in compoundStatus.Statuses)
                    InternalAddStatus(subStatus);

            foreach (var handler in Handlers)
                if (handler.CanHandle(status))
                    handler.AddStatus(status);
        }

        private void InternalRemoveStatus(IStatus status)
        {
            if (status is ICompoundStatus compoundStatus)
                foreach (var subStatus in compoundStatus.Statuses)
                    InternalRemoveStatus(subStatus);

            foreach (var handler in Handlers)
                if (handler.CanHandle(status))
                    handler.RemoveStatus(status);
        }
    }
}