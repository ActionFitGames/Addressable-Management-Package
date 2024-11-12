
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal class FetchLoader : NeedResourceSystemRegistry, IFetchLoader
    {
        public void LoadAsset<T>(AssetKey assetKey, ProvideLoadOperation<T> operation) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetKey.Runtime);
            handle.Completed += asyncHandle => HandleLoadCompletion(asyncHandle, assetKey, operation);
        }

        private void HandleLoadCompletion<T>(AsyncOperationHandle<T> handle, AssetKey assetKey, ProvideLoadOperation<T> operation) 
            where T : Object
        {
            if (!IsLoadSuccessful(handle, assetKey, out var error))
            {
                operation.SetError(error);
                return;
            }

            ProcessLoadedAsset(handle.Result, assetKey, typeof(T), operation);
        }

        private bool IsLoadSuccessful<T>(AsyncOperationHandle<T> handle, AssetKey assetKey, out Exception error) 
            where T : Object
        {
            error = null;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return true;
            }
            
            error = new Exception($"Failed to load asset: {assetKey.Primary}");
            return false;
        }

        private void ProcessLoadedAsset<T>(T loadedAsset, AssetKey assetKey, Type assetType, ProvideLoadOperation<T> operation) 
            where T : Object
        {
            if (TryAddAsDependency(loadedAsset, assetKey, assetType))
            {
                operation.SetResult(loadedAsset);
                return;
            }

            if (TryCreateNewAssetReference(loadedAsset, assetKey))
            {
                operation.SetResult(loadedAsset);
                return;
            }

            operation.SetError(new Exception($"Failed to cache loaded asset: {assetKey.Primary}"));
        }

        private bool TryAddAsDependency<T>(T loadedAsset, AssetKey assetKey, Type assetType) where T : Object
        {
            if (!Registry.LoadedAssetMap.TryGetValue(assetKey, out var existingReference))
            {
                return false;
            }

            existingReference.Dependencies ??= new List<AssetReferenceStDependencies>();
            existingReference.Dependencies.Add(new AssetReferenceStDependencies(assetType, loadedAsset));
            ++existingReference.ReferenceCount;
            return true;
        }

        private bool TryCreateNewAssetReference<T>(T loadedAsset, AssetKey assetKey) where T : Object
        {
            var newReference = new AssetReferenceSt
            {
                IsPrepare = false,
                ReferenceCount = 1,
                Asset = loadedAsset
            };

            return Registry.LoadedAssetMap.TryAdd(assetKey, newReference);
        }

        public FetchLoader(ResourceSystemRegistry resourceSystemRegistry) : base(resourceSystemRegistry) { }
    }
}