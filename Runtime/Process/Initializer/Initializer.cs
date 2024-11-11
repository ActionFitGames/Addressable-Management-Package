
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal class Initializer : NeedResourceSystemRegistry, IInitializer
    {
        private readonly Dictionary<string, (string primary, string runtime, Type type)> _tempKeys = new();

        public async Task<IResourceLocator> InitializeSystem(Action<bool> onStrategy, bool autoReleaseHandle = true)
        {
            try
            {
                var handle = Addressables.InitializeAsync(false);
                var result = await handle.Task;
                
                MapAssetKeys(result);
                CompleteInitialization(handle, onStrategy, autoReleaseHandle);
                
                return result;
            }
            catch (Exception exception)
            {
                onStrategy.Invoke(false);
                AddressableLog.Error($"An error occurred during initialization System. {exception.Message}");
                throw;
            }
        }

        private void MapAssetKeys(IResourceLocator resourceLocator)
        {
            _tempKeys.Clear();
            
            foreach (var key in resourceLocator.Keys)
            {
                ProcessResourceLocatorKey(resourceLocator, key);
            }
        }

        private void ProcessResourceLocatorKey(IResourceLocator resourceLocator, object key)
        {
            if (!TryGetLocations(resourceLocator, key, out var locations))
            {
                return;
            }

            var keyStr = key.ToString();
            var isGuid = Guid.TryParse(keyStr, out _);

            foreach (var location in locations)
            {
                ProcessLocation(location, keyStr, isGuid);
            }
        }

        private bool TryGetLocations(IResourceLocator resourceLocator, object key, out IList<IResourceLocation> locations)
        {
            return resourceLocator.Locate(key, typeof(Object), out locations);
        }

        private void ProcessLocation(IResourceLocation location, string keyStr, bool isGuid)
        {
            var internalId = location.InternalId;
            var keyInfo = GetOrCreateKeyInfo(internalId, location.ResourceType);
            
            keyInfo = UpdateKeyInfo(keyInfo, keyStr, isGuid);
            _tempKeys[internalId] = keyInfo;

            TryCreateAndAddAssetKey(internalId, keyInfo);
        }

        private (string primary, string runtime, Type type) GetOrCreateKeyInfo(string internalId, Type resourceType)
        {
            return _tempKeys.TryGetValue(internalId, out var existingKeyInfo) 
                ? existingKeyInfo 
                : (null, null, resourceType);
        }

        private (string primary, string runtime, Type type) UpdateKeyInfo(
            (string primary, string runtime, Type type) keyInfo, string keyStr, bool isGuid)
        {
            return isGuid
                ? (keyInfo.primary, keyStr, keyInfo.type)
                : (keyStr, keyInfo.runtime, keyInfo.type);
        }

        private void TryCreateAndAddAssetKey(string internalId, (string primary, string runtime, Type type) keyInfo)
        {
            if (string.IsNullOrEmpty(keyInfo.primary) || string.IsNullOrEmpty(keyInfo.runtime))
            {
                return;
            }

            var assetKey = new AssetKey(
                keyInfo.primary,
                keyInfo.runtime,
                internalId,
                keyInfo.type
            );

            Registry.CachedAssetKeys.Add(assetKey);
        }

        private void CompleteInitialization(AsyncOperationHandle handle, Action<bool> onStrategy, bool autoReleaseHandle)
        {
            onStrategy.Invoke(true);
            if (autoReleaseHandle)
            {
                Addressables.Release(handle);
                _tempKeys.Clear();
            }
        }

        public Initializer(ResourceSystemRegistry resourceSystemRegistry) : base(resourceSystemRegistry) { }
    }
}