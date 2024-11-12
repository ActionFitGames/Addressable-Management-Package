using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class Provider
    {
        /// <summary>
        /// Retrieves an asset asynchronously using the provided key.
        /// Checks cache first, then initiates loading if not found.
        /// </summary>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <param name="assetKeyOrigin">Key to identify the asset (AssetReference or string)</param>
        /// <returns>Operation handling the asset loading process</returns>
        public IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
        {
            var operation = new ProvideLoadOperation<T>();

            try
            {
                if (!TryResolveAssetKey(assetKeyOrigin, typeof(T), out var assetKey))
                {
                    operation.SetError(CreateKeyTypeError(assetKeyOrigin));
                    return operation;
                }

                ValidateGameObjectRequest<T>();
                
                if (TryGetFromCache<T>(assetKey, out var cachedAsset))
                {
                    operation.SetResult(cachedAsset);
                    return operation;
                }

                _fetchLoader.LoadAsset(assetKey, operation);
            }
            catch (Exception exception)
            {
                operation.SetError(exception);
            }

            return operation;
        }
        
        /// <summary>
        /// Attempts to get an asset synchronously from the cache.
        /// Returns false if the asset isn't cached or any error occurs.
        /// </summary>
        /// <typeparam name="T">The type of asset to retrieve</typeparam>
        /// <param name="assetKeyOrigin">Key to identify the asset</param>
        /// <param name="resultAsset">The retrieved asset if successful</param>
        public bool TryGetAssetSafety<T>(object assetKeyOrigin, out T resultAsset) where T : Object
        {
            resultAsset = null;
            
            try
            {
                if (!TryResolveAssetKey(assetKeyOrigin, typeof(T), out var assetKey))
                {
                    return false;
                }

                ValidateGameObjectRequest<T>();
                return TryGetFromCache(assetKey, out resultAsset);
            }
            catch (Exception exception)
            {
                LogGetSafetyError(exception);
                return false;
            }
        }

        #region Helper Methods
        
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

        private static void LogGetSafetyError(Exception exception) =>
            AddressableLog.Error($"Exception with GetSafety: {exception.Message}");
        
        #endregion
    }
}