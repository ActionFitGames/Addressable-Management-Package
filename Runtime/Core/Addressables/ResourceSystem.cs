
using System;

namespace ActionFit.Framework.Addressable
{
    internal partial class ResourceSystem : InternalSingleton<ResourceSystem>, IDisposable
    {
        private readonly ResourceSystemRegistry _resourceSystemRegistry;
        private readonly IProcess _process;
        private readonly IProvider _provider;

        private ResourceSystemSettingSO _setting;
        
        internal static bool IsActivateInitialize { get; set; }
        
        #region Constructor
        
        public ResourceSystem()
        {
            _resourceSystemRegistry = new ResourceSystemRegistry();
            _process = new Process(_resourceSystemRegistry);
            _provider = new Provider(_resourceSystemRegistry, _process.FetchLoader);
            
            _setting = ResourceSystemSettingSO.Instance;
        }

        #endregion
        
        public void Dispose()
        {
            _resourceSystemRegistry.Dispose();
            _process.Dispose();
            _provider.Dispose();
        }
    }
}

