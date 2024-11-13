
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public enum WhenToRelease
    {
        OnDestroy,
        OnDisable
    }
    
    public abstract class ResourceUsableBehavior : MonoBehaviour
    {
        private IProvider Resource { get; set; }
        private readonly List<AssetKey> _loadedAssetKeys = new();

        [SerializeField] private WhenToRelease _toRelease;

        protected virtual void Awake()
        {
            Resource = ResourceSystem.GetProvider();
        }

        protected void OnDisable()
        {
            if (_toRelease != WhenToRelease.OnDisable)
            {
                return;
            }

            ReleaseAllLoadedAssets();
        }

        protected virtual void OnDestroy()
        {
            if (_toRelease != WhenToRelease.OnDestroy)
            {
                return;
            }
            
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

        #region Manually Release

        protected void ReleaseManually(AssetReference assetKeyOrigin)
        {
            var loadedKey = FindLoadedAssetKey(null, assetKeyOrigin.RuntimeKey.ToString());
            TryReleaseAsset(loadedKey);
        }

        protected void ReleaseManually(string primaryKey)
        {
            var loadedKey = FindLoadedAssetKey(primaryKey, null);
            TryReleaseAsset(loadedKey);
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
        
        private AssetKey FindLoadedAssetKey(string primaryKey, string runtimeKey)
        {
            var accessKey = new AssetKey(primaryKey, runtimeKey, null, null);
            return _loadedAssetKeys.FirstOrDefault(key => key.Equals(accessKey));
        }

        private void TryReleaseAsset(AssetKey assetKey)
        {
            if (assetKey.Equals(default))
            {
                AddressableLog.Error("Can't find loaded asset Key.");
                return;
            }

            try
            {
                ResourceSystem.Instance.ReleaseAsset(assetKey);
                _loadedAssetKeys.Remove(assetKey);
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Failed to release asset {assetKey}: {exception.Message}");
            }
        }
    }
}