
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader : NeedResourceSystemRegistry, IPrepareLoader
    {
        public Dictionary<AssetLabelReference, HashSet<AssetKey>> LoadedLabelKeyMap { get; }
        
        private readonly LoadProgressTracker _progressTracker;
        private readonly List<AsyncOperationHandle> _operationHandles;

        public PrepareLoader(ResourceSystemRegistry resourceSystemRegistry) : base(resourceSystemRegistry)
        {
            LoadedLabelKeyMap = new Dictionary<AssetLabelReference, HashSet<AssetKey>>();
            _progressTracker = new LoadProgressTracker();
            _operationHandles = new List<AsyncOperationHandle>();
        }

        public void ReleaseLabel(AssetLabelReference releaseLabelRef)
        {
            if (!LoadedLabelKeyMap.TryGetValue(releaseLabelRef, out var hashAssetKeys))
            {
                AddressableLog.Error("Can't Release the AssetLabel with PrepareLoader.");
                return;
            }
            
            hashAssetKeys.Clear();
            LoadedLabelKeyMap.Remove(releaseLabelRef);
        }

        public void Dispose()
        {
            LoadedLabelKeyMap.Clear();
            _operationHandles.Clear();
        }
    }
}