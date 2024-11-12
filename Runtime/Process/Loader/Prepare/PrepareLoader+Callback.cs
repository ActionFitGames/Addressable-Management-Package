
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader
    {
        /// <summary>
        /// Handles the completion of an asset load operation.
        /// Validates the operation result and processes the loaded asset.
        /// </summary>
        private void HandleAssetLoadComplete(AsyncOperationHandle handle, AssetLabelReference label, IResourceLocation location)
        {
            if (!IsValidLoadResult(handle, out var loadedObject))
            {
                return;
            }
            
            ProcessLoadedAsset(label, loadedObject, location);
            _progressTracker.IncrementLoadedCount();
        }

        /// <summary>
        /// Processes a loaded asset by either adding it as a dependency to an existing asset
        /// or registering it as a new main asset.
        /// </summary>
        private void ProcessLoadedAsset(AssetLabelReference label, Object loadedObject, IResourceLocation location)
        {
            if (!TryGetAssetKey(location, loadedObject, out var assetKey))
            {
                return;
            }

            if (TryAddAsDependency(assetKey, location, loadedObject))
            {
                return;
            }

            RegisterNewAsset(label, assetKey, loadedObject, location);
        }

        /// <summary>
        /// Retrieves the valid asset key from the cache using the location's internal ID.
        /// Returns false if the key mapping doesn't exist.
        /// </summary>
        private bool TryGetAssetKey(IResourceLocation location, Object loadedObject, out AssetKey validAssetKey)
        {
            var accessKey = CreateAccessKey(location.InternalId);
            if (Registry.CachedAssetKeys.TryGetValue(accessKey, out validAssetKey))
            {
                return true;
            }
            
            LogKeyMappingError(loadedObject);
            return false;
        }

        /// <summary>
        /// Creates a new asset reference and adds it to the registry.
        /// Returns false if the addition fails.
        /// </summary>
        private bool TryAddAssetToRegistry(AssetKey assetKey, Object loadedObject)
        {
            var reference = CreateAssetReference(loadedObject);
            return Registry.LoadedAssetMap.TryAdd(assetKey, reference);
        }

        /// <summary>
        /// Maps an asset key to its corresponding label and logs the result.
        /// </summary>
        private void MapAssetKeyToLabel(AssetLabelReference label, AssetKey assetKey, Object loadedObject)
        {
            if (!LoadedLabelKeyMap.TryGetValue(label, out var assetKeys))
            {
                LogLabelMappingError(label);
                return;
            }

            assetKeys.Add(assetKey);
            LogSuccessfulMapping(loadedObject, label);
        }

        #region Helper Methods
        
        private bool IsValidLoadResult(AsyncOperationHandle handle, out Object loadedObject)
        {
            loadedObject = null;
            return handle.Status == AsyncOperationStatus.Succeeded 
                   && (loadedObject = handle.Result as Object) != null;
        }

        private static AssetKey CreateAccessKey(string internalId) => new(null, null, internalId, null);

        private static AssetCustomRef CreateAssetReference(Object asset) => new AssetCustomRef
        {
            ReferenceCount = 0,
            IsPrepare = true,
            Asset = asset
        };

        private bool TryAddAsDependency(AssetKey assetKey, IResourceLocation location, Object loadedObject)
        {
            if (!Registry.LoadedAssetMap.TryGetValue(assetKey, out var assetReferenceSt))
            {
                return false;
            }

            assetReferenceSt.Dependencies ??= new List<AssetCustomRefDependencies>();
            assetReferenceSt.Dependencies.Add(new AssetCustomRefDependencies(location.ResourceType, loadedObject));
            return true;
        }

        private void RegisterNewAsset(AssetLabelReference label, AssetKey assetKey, Object loadedObject, IResourceLocation location)
        {
            if (!TryAddAssetToRegistry(assetKey, loadedObject))
            {
                return;
            }
            
            MapAssetKeyToLabel(label, assetKey, loadedObject);
            ResourceSystemEventHandler.OnPrepareLoadedCallback?.Invoke(loadedObject, location);
        }

        private static void LogKeyMappingError(Object loadedObject)
            => AddressableLog.Warning($"'{loadedObject.name}' is not KeyMapping. ERROR 200");

        private static void LogLabelMappingError(AssetLabelReference label)
            => AddressableLog.Error($"Label reference '{label.labelString}' not found in LoadedLabelKeyMap");

        private static void LogSuccessfulMapping(Object loadedObject, AssetLabelReference label)
            => AddressableLog.Debug($"Load Complete '{loadedObject.name}' and mapped to label '{label.labelString}'");
        
        #endregion
    }
}