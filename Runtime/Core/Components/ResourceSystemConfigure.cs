
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    [RequireComponent(typeof(ResourceSystemComponent))]
    public class ResourceSystemConfigure : LifeCycleBehavior
    {
        private ResourceSystemSettingSO _cachedInstance;
        
        protected override void InitializeInternal()
        {
            _cachedInstance = ResourceSystemSettingSO.Instance;
            if (_cachedInstance.UseDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
