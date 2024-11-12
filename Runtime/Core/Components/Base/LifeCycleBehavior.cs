using System;
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    public abstract class LifeCycleBehavior : MonoBehaviour
    {
        #region Fields

        internal static ResourceSystem ResourceSystem;

        internal static event Action<float> OnUpdateMainThread = delegate { };

        #endregion
        
        private void Awake()
        {
            ResourceSystem ??= ResourceSystem.Activate();

            InitializeInternal();
        }

        private void Update()
        {
            OnUpdateMainThread.Invoke(Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (ResourceSystem == null)
            {
                return;
            }
            
            ResourceSystem.Dispose();
            ResourceSystem = null;
        }

        protected abstract void InitializeInternal();
    }
}