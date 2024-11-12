
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    /// <summary>
    /// Process Internal
    /// </summary>
    internal partial class Provider
    {
        private void ValidateGameObjectRequest<T>() where T : Object
        {
            if (ValidateExcludeType(typeof(T), typeof(GameObject)))
            {
                throw new InvalidOperationException("Can't get the asset, You can use Instantiate.");
            }
        }

        private T GetCachedAsset<T>(AssetKey assetKey) where T : Object
        {
            if (!Registry.LoadedAssetMap.TryGetValue(assetKey, out var assetRef))
            {
                return null;
            }
            
            if (assetRef.Asset is T mainAsset)
            {
                ++assetRef.ReferenceCount;
                return mainAsset;
            }

            var dependency = assetRef.Dependencies?.FirstOrDefault(d => d.Asset is T);
            if (dependency?.Asset is not T dependentAsset)
            {
                return null;
            }
            
            ++assetRef.ReferenceCount;
            return dependentAsset;

        }

        private bool ValidateExcludeType(Type requestType, Type excludeType)
        {
            return requestType == excludeType;
        }
    }
}