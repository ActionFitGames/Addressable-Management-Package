
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ActionFit.Framework.Addressable
{
    /// <summary>
    /// Release Parts
    /// </summary>
    internal partial class ResourceSystem
    {
        #region Fields
        
        private readonly Queue<AssetKey> _zeroRefKeys = new();
        private float _lastCleanupTime;
        private bool _isProcessingRelease;
        
        #endregion

        #region Asset Release

        internal void ReleaseAsset(AssetKey key)
        {
            if (!_resourceSystemRegistry.LoadedAssetMap.TryGetValue(key, out var assetRef))
            {
                return;
            }

            --assetRef.ReferenceCount;
            
            if (assetRef is { IsPrepare: true, ReferenceCount: <= 0 })
            {
                assetRef.ReferenceCount = 0;
                return;
            }

            if (assetRef.ReferenceCount > 0)
            {
                return;
            }
            
            if (!_zeroRefKeys.Contains(key))
            {
                _zeroRefKeys.Enqueue(key);
            }
            
            if (_zeroRefKeys.Count >= _setting.BatchReleaseThreshold)
            {
                ProcessPendingReleases();
            }
            else
            {
                ProcessSingleRelease(key);
            }
        }

        internal void ReleaseLabel(AssetLabelReference labelReference)
        {
            if (!_resourceSystemRegistry.LoadedAssetMap.Any())
            {
                return;
            }

            if (!_process.PrepareLoader.LoadedLabelKeyMap.TryGetValue(labelReference, out var labelKeys))
            {
                AddressableLog.Error($"Can't find label '{labelReference.labelString}'");
                return;
            }

            if (_isProcessingRelease)
            {
                return;
            }

            try
            {
                _isProcessingRelease = true;
                
                foreach (var preparedKey in labelKeys)
                {
                    ReleaseKey(preparedKey);
                }
                
                _process.PrepareLoader.ReleaseLabel(labelReference);
                
                Resources.UnloadUnusedAssets();
                GC.Collect();
                _lastCleanupTime = Time.realtimeSinceStartup;
            }
            finally
            {
                _isProcessingRelease = false;
            }
        }

        private void ReleaseKey(AssetKey assetKey)
        {
            if (!_resourceSystemRegistry.LoadedAssetMap.TryGetValue(assetKey, out var assetReferenceSt))
            {
                return;
            }

            try
            {
                assetReferenceSt.Asset = null;
                assetReferenceSt.Dependencies?.Clear();
                assetReferenceSt.Dependencies = null;
                
                _resourceSystemRegistry.LoadedAssetMap.Remove(assetKey);
                _resourceSystemRegistry.CachedAssetKeys.Remove(assetKey);
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Failed to release asset : {exception.Message}");
            }
        }

        #endregion

        #region Process Management

        private void ProcessSingleRelease(AssetKey key)
        {
            if (_isProcessingRelease)
            {
                return;
            }
            
            try
            {
                _isProcessingRelease = true;
                ReleaseKey(key);
            }
            finally
            {
                _isProcessingRelease = false;
            }
        }

        private void ProcessPendingReleases(bool force = false)
        {
            if (!force && _zeroRefKeys.Count < _setting.BatchReleaseThreshold)
            {
                return;
            }

            if (_isProcessingRelease)
            {
                return;
            }

            try
            {
                _isProcessingRelease = true;
                
                while (_zeroRefKeys.Count > 0)
                {
                    var key = _zeroRefKeys.Dequeue();
                    ReleaseKey(key);
                }

                PerformMemoryCleanup(force);
            }
            catch (Exception e)
            {
                AddressableLog.Error($"Failed to process pending releases: {e.Message}");
            }
            finally
            {
                _isProcessingRelease = false;
            }
        }

        private void PerformMemoryCleanup(bool force)
        {
            var currentTime = Time.realtimeSinceStartup;
            if (!force && currentTime - _lastCleanupTime < _setting.MemoryCleanupInterval)
            {
                return;
            }

            Resources.UnloadUnusedAssets();
            GC.Collect();
            
            _lastCleanupTime = currentTime;
        }

        #endregion

        #region Zero Key Util

        internal void RemoveFromZeroRefKeys(AssetKey key)
        {
            if (_zeroRefKeys.Count == 0)
            {
                return;
            }
            
            var tempList = new List<AssetKey>(_zeroRefKeys);
            _zeroRefKeys.Clear();
            
            foreach (var queuedKey in tempList.Where(k => !k.Equals(key)))
            {
                _zeroRefKeys.Enqueue(queuedKey);
            }
        }

        #endregion
    }
}