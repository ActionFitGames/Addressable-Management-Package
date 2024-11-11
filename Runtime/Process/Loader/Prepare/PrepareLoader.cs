
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader : NeedResourceSystemRegistry, IPrepareLoader
    {
        internal Dictionary<AssetLabelReference, HashSet<AssetKey>> LoadedLabelKeyMap { get; }
        
        private readonly LoadProgressTracker _progressTracker;
        private readonly List<AsyncOperationHandle> _operationHandles;

        public PrepareLoader(ResourceSystemRegistry resourceSystemRegistry) : base(resourceSystemRegistry)
        {
            LoadedLabelKeyMap = new Dictionary<AssetLabelReference, HashSet<AssetKey>>();
            _progressTracker = new LoadProgressTracker();
            _operationHandles = new List<AsyncOperationHandle>();
        }

        public void Dispose()
        {
            LoadedLabelKeyMap.Clear();
        }
    }
}