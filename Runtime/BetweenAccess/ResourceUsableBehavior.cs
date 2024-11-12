
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public abstract class ResourceUsableBehavior : MonoBehaviour
    {
        private IProvider Resource { get; set; }
        private readonly List<AssetKey> _loadedAssetKeys = new();

        protected virtual void Awake()
        {
            Resource = ResourceSystem.GetProvider();
        }

        protected virtual void OnDestroy()
        {
            ReleaseAllLoadedAssets();
        }

        #region Get Asset

        protected IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
        {
            var operation = Resource.GetAsset<T>(assetKeyOrigin, out var assetKey);
            operation.OnComplete(asset =>
            {
                if (asset != null && assetKey != default)
                {
                    _loadedAssetKeys.Add(assetKey);
                }
            });

            return operation;
        }

        protected bool TryGetAsset<T>(object assetKeyOrigin, out T result) where T : Object
        {
            if (!Resource.TryGetAssetSafety(assetKeyOrigin, out result, out var assetKey))
            {
                return false;
            }
            
            _loadedAssetKeys.Add(assetKey);
            return true;
        }

        #endregion

        #region Instantiate

        protected IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Transform parent = null)
        {
            var operation = Resource.Instantiate(assetKeyOrigin, parent);
            return operation;
        }
        
        protected IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var operation = Resource.Instantiate(assetKeyOrigin, position, rotation, parent);
            return operation;
        }

        #endregion

        private void ReleaseAllLoadedAssets()
        {
            if (_loadedAssetKeys.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var loadedAssetKey in _loadedAssetKeys)
                {
                    try
                    {
                        ResourceSystem.Instance.ReleaseAsset(loadedAssetKey);
                    }
                    catch (Exception exception)
                    {
                        AddressableLog.Error($"Failed to release asset {loadedAssetKey}: {exception.Message}");
                    }
                }
            }
            finally
            {
                _loadedAssetKeys.Clear();
            }
        }
    }
}