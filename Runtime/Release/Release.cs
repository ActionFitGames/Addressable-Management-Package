
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace ActionFit.Framework.Addressable
{
    internal class Release : NeedResourceSystemRegistry
    {
        #region Fields

        private const int BatchReleaseThreshold = 10;
        
        private IPrepareLoader _prepareLoader;
        
        internal readonly HashSet<AssetKey> PendingReleaseKeys = new();
        internal readonly Queue<AssetKey> ZeroRefKeys = new();
        

        #endregion
        

        public void ReleaseLabel(AssetLabelReference labelReference)
        {
            if (!Registry.LoadedAssetMap.Any())
            {
                return;
            }

            if (!_prepareLoader.LoadedLabelKeyMap.TryGetValue(labelReference, out var labelKeys))
            {
                AddressableLog.Error($"Can't find label '{labelReference.labelString}'");
                return;
            }

            var keysToRemove = labelKeys.ToList();
        }

        #region internal Release

        private void ReleaseKey(AssetKey assetKey)
        {
            if (!Registry.LoadedAssetMap.TryGetValue(assetKey, out var assetReferenceSt))
            {
                return;
            }

            try
            {
                assetReferenceSt.Asset = null;
                assetReferenceSt.Dependencies.Clear();
                assetReferenceSt.Dependencies = null;
                
                Registry.LoadedAssetMap.Remove(assetKey);
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Failed to force release asset : {exception.Message}");
            }
        }

        #endregion

        #region Process Util

        private void ProcessUnusedAsset(bool force = false)
        {
            if (!force && ZeroRefKeys.Count < BatchReleaseThreshold)
            {
                return;
            }

            try
            {
                while (ZeroRefKeys.Count > 0)
                {
                    var assetKey = ZeroRefKeys.Dequeue();
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        public Release(ResourceSystemRegistry resourceSystemRegistry, IPrepareLoader prepareLoader)
            : base(resourceSystemRegistry)
        {
            _prepareLoader = prepareLoader;
        }
    }
}