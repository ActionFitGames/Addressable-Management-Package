
using System;

namespace ActionFit.Framework.Addressable
{
    internal partial class ResourceSystem : InternalSingleton<ResourceSystem>, IDisposable
    {
        private readonly ResourceSystemRegistry _resourceSystemRegistry;
        private readonly Process _process;
        
        internal static bool IsActivateInitialize { get; set; }
        
        #region Constructor
        
        public ResourceSystem()
        {
            _resourceSystemRegistry = new ResourceSystemRegistry();
            
            _process = new Process(_resourceSystemRegistry);
        }

        #endregion
        
        public void Dispose()
        {
            _resourceSystemRegistry.Dispose();
            _process.Dispose();
        }
    }
}

