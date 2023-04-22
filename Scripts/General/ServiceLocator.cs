using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
    public interface IService { }

    /// <summary>
    /// Contains a set of services that can be fetches anywhere at any time. Services are mapped to specific types that can look them up.
    /// </summary>
    public partial class ServiceLocator : Node
    {
        /// <summary>
        /// Global instance of the service locator
        /// </summary>
        public static ServiceLocator Global { get; set; }
        [ExportCategory("Settings")]
        [Export]
        public bool IsGlobal { get; set; } = false;

        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();
        public IReadOnlyDictionary<Type, IService> Services => services;

        public override void _Ready()
        {
            if (IsGlobal)
            {
                if (Global != null)
                {
                    QueueFree();
                    return;
                }
                Global = this;
                Reparent(GetTree().Root);
            }

            foreach (Node child in GetChildren())
            {
                if (child is IService service)
                    AddService(service);
            }
        }

        /// <summary>
        /// Adds a node service and reparents it to the service locator so it persists between scene loads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public void AddPersistentService<T>(T service) where T : Node, IService
        {
            // If the there is an existing service and it's a persistent service (aka. it's also a child of this service locator),
            // then we want to queue free it and replace it with this new service.
            if (services.TryGetValue(typeof(T), out IService existingService) && existingService is Node serviceNode && serviceNode.GetParent() == this)
                serviceNode.QueueFree();
            AddService(service);
            service.Reparent(this);
        }

        /// <summary>
        /// Tries to add a service. If a service was already registered to this type, this returns false. Otherwise the service is added and reparented to the service locator so it persists between scene loads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryAddPersistentService<T>(T service) where T : Node, IService
        {
            var result = TryAddService(service);
            if (result)
                service.Reparent(this);
            return result;
        }

        /// <summary>
        /// Tries to add a service. If a service was already registered to this type, this returns false. Otherwise the service is added and returns true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryAddService<T>(T service) where T : class, IService
        {
            if (services.ContainsKey(typeof(T)))
                return false;
            services[typeof(T)] = service;
            return true;
        }

        /// <summary>
        /// Adds a service. If a service was already registered to this type, the old service is replaced by the new service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public void AddService<T>(T service) where T : class, IService
        {
            services[typeof(T)] = service;
        }


        /// <summary>
        /// Returns a service if it exists in the service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : class, IService
        {
            if (services.TryGetValue(typeof(T), out IService service))
                return service as T;
            return null;
        }

        /// <summary>
        /// Checks if Service was registered to a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasService<T>() where T : class, IService
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Removes a service mapping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveService<T>() where T : class, IService
        {
            return services.Remove(typeof(T));
        }

        /// <summary>
        /// Removes and frees a service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveAndFreeService<T>() where T : Node, IService
        {
            if (services.TryGetValue(typeof(T), out IService service))
            {
                if (service is Node node)
                    node.QueueFree();
                services.Remove(typeof(T));
                return true;
            }
            return false;
        }
    }
}