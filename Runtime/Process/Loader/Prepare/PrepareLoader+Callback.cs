
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader
    {
        private void HandleAssetLoadComplete(AsyncOperationHandle handle, AssetLabelReference label, IResourceLocation location)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            var loadedObject = handle.Result as Object;
            if (!loadedObject)
            {
                return;
            }

            ProcessLoadedAsset(label, loadedObject, location);
            _progressTracker.IncrementLoadedCount();
        }

        private void ProcessLoadedAsset(AssetLabelReference label, Object loadedObject, IResourceLocation location)
        {
            if (!TryGetAssetKey(location, loadedObject, out var assetKey))
            {
                return;
            }

            if (!TryAddAssetToRegistry(assetKey, loadedObject))
            {
                return;
            }
            
            MapAssetKeyToLabel(label, assetKey, loadedObject);
            ResourceSystemEventHandler.OnPrepareLoadedCallback?.Invoke(loadedObject, location);
        }

        private bool TryGetAssetKey(IResourceLocation location, Object loadedObject, out AssetKey assetKey)
        {
            var accessKey = new AssetKey(null, null, location.InternalId, null);
            if (Registry.CachedAssetKeys.TryGetValue(accessKey, out assetKey))
            {
                return true;
            }
            
            AddressableLog.Error($"'{loadedObject.name}' is not KeyMapping. ERROR 200");
            return false;
        }

        private bool TryAddAssetToRegistry(AssetKey assetKey, Object loadedObject)
        {
            var reference = new AssetReferenceSt
            {
                ReferenceCount = 0,
                IsPrepare = true,
                Asset = loadedObject
            };

            return Registry.LoadedAssetMap.TryAdd(assetKey, reference);
        }

        private void MapAssetKeyToLabel(AssetLabelReference label, AssetKey assetKey, Object loadedObject)
        {
            if (LoadedLabelKeyMap.TryGetValue(label, out var assetKeys))
            {
                assetKeys.Add(assetKey);
                AddressableLog.Debug($"Load Complete '{loadedObject.name}' and mapped to label '{label.labelString}'");
            }
            else
            {
                AddressableLog.Error($"Label reference '{label.labelString}' not found in LoadedLabelKeyMap");
            }
        }
    }
}