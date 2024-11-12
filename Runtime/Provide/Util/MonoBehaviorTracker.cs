
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    public sealed class MonoBehaviorTracker : MonoBehaviour
    {
        public delegate void DelegateDestroyed(MonoBehaviorTracker trackingBehavior);
        public event DelegateDestroyed OnDestroyed;
        
        internal AssetKey Key { get; set; }
        
        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}