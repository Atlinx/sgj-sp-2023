using Godot;
using System.Collections.Generic;

namespace Game
{
    public abstract partial class HashSetStatusHandler : Node, IStatusHandler
    {
        public HashSet<IStatus> HandledStatuses { get; private set; } = new HashSet<IStatus>();
        public abstract bool CanHandle(IStatus status);
        public virtual void AddStatus(IStatus status)
        {
            HandledStatuses.Add(status);
        }
        public virtual void RemoveStatus(IStatus status)
        {
            HandledStatuses.Remove(status);
        }
    }
}