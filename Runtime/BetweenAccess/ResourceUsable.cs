
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public abstract class ResourceUsable : IDisposable
    {
        private IProvider Resource { get; set; }
        private readonly List<AssetKey> _loadedAssetKeys = new();
        private bool _isDisposed;

        protected ResourceUsable()
        {
            Resource = ResourceSystem.GetProvider();
        }

        #region Get Asset

        protected IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
        {
            ThrowIfDisposed();
            
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
            ThrowIfDisposed();
            
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
            ThrowIfDisposed();
            return Resource.Instantiate(assetKeyOrigin, parent);
        }
        
        protected IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            ThrowIfDisposed();
            return Resource.Instantiate(assetKeyOrigin, position, rotation, parent);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                ReleaseAllLoadedAssets();
            }

            _isDisposed = true;
        }

        ~ResourceUsable()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
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