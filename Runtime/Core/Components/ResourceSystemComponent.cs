
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    [RequireComponent(typeof(ResourceSystemConfigure))]
    public class ResourceSystemComponent : LifeCycleBehavior
    {
        #region Fields

        private static readonly object QueueLock = new();
        private static volatile bool _isProcessingQueue;
        private static readonly Dictionary<MonoBehaviour, Queue<Action>> ComponentActionQueues = new();

        internal static event Action<float> OnUpdateMainThread = delegate { };
        internal static event Action<float> OnFixedUpdateMainThread = delegate { };

        #endregion

        protected override void InitializeInternal() { }

        private void Update()
        {
            ProcessPendingQueues();
            OnUpdateMainThread.Invoke(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            OnFixedUpdateMainThread.Invoke(Time.fixedDeltaTime);
        }

        private static void ProcessPendingQueues()
        {
            if (!ResourceSystem.IsActivateInitialize || _isProcessingQueue)
                return;

            Dictionary<MonoBehaviour, List<Action>> actionsToProcess = null;

            lock (QueueLock)
            {
                if (ComponentActionQueues.Count == 0)
                    return;

                _isProcessingQueue = true;
                
                // 모든 컴포넌트의 대기 중인 액션들을 복사
                actionsToProcess = new Dictionary<MonoBehaviour, List<Action>>();
                foreach (var kvp in ComponentActionQueues
                             .Where(kvp => kvp.Value.Count > 0))
                {
                    actionsToProcess[kvp.Key] = new List<Action>(kvp.Value);
                    kvp.Value.Clear();
                }
            }

            try
            {
                foreach (var (component, value) in actionsToProcess)
                {
                    if (!component) 
                    {
                        RemoveComponentQueue(component);
                        continue;
                    }

                    foreach (var action in value)
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception exception)
                        {
                            AddressableLog.Error($"Error executing queued action: {exception.Message}");
                        }
                    }
                }
            }
            finally
            {
                _isProcessingQueue = false;
            }
        }

        public static void EnqueueAction(MonoBehaviour component, Action action)
        {
            if (action == null || !component)
            {
                return;
            }

            if (ResourceSystem.IsActivateInitialize)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception exception)
                {
                    AddressableLog.Error($"Error executing immediate action: {exception}");
                }
                return;
            }

            lock (QueueLock)
            {
                if (!ComponentActionQueues.TryGetValue(component, out var queue))
                {
                    queue = new Queue<Action>();
                    ComponentActionQueues[component] = queue;
                }
                queue.Enqueue(action);
            }
        }

        private static void RemoveComponentQueue(MonoBehaviour component)
        {
            lock (QueueLock)
            {
                ComponentActionQueues.Remove(component);
            }
        }

        public static void CleanupComponentQueue(MonoBehaviour component)
        {
            if (!component)
            {
                return;
            }

            RemoveComponentQueue(component);
        }
    }
}