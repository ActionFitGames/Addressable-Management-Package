
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        
        /// <summary>
        /// Resolves various key types into a valid AssetKey.
        /// </summary>
        private bool TryResolveAssetKey(object assetKeyOrigin, Type resourceType, out AssetKey resolvedKey)
        {
            resolvedKey = default;

            if (assetKeyOrigin == null)
            {
                return false;
            }

            var tempKey = CreateAssetKey(assetKeyOrigin, resourceType);
            if (!tempKey.IsValid)
            {
                return false;
            }

            resolvedKey = Registry.CachedAssetKeys.FirstOrDefault(key => key.Equals(tempKey));
            return resolvedKey != default;
        }

        private static AssetKey CreateAssetKey(object keyOrigin, Type resourceType) => keyOrigin switch
        {
            AssetReference assetRef => new AssetKey(null, assetRef.RuntimeKey.ToString(), null, resourceType),
            string primaryKey => new AssetKey(primaryKey, null, null, resourceType),
            _ => default
        };

        private static ArgumentException CreateKeyTypeError(object assetKeyOrigin) =>
            new($"Unsupported asset key type: {assetKeyOrigin?.GetType()}, " +
                "if string or AssetReference, not valid Addressables");

        private bool TryGetFromCache<T>(AssetKey key, out T asset) where T : Object
        {
            asset = GetCachedAsset<T>(key);
            return asset != null;
        }
    }
}