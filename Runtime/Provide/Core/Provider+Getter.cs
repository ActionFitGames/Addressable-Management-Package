
using System;
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
        /// <param name="returnKey"></param>
        /// <returns>Operation handling the asset loading process</returns>
        public IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin, out AssetKey returnKey) where T : Object
        {
            var operation = new ProvideLoadOperation<T>();
            AssetKey resultKey = default;

            try
            {
                if (!TryResolveAssetKey(assetKeyOrigin, typeof(T), out var assetKey))
                {
                    operation.SetError(CreateKeyTypeError(assetKeyOrigin));
                    returnKey = default;
                    return operation;
                }
                
                ValidateGameObjectRequest<T>();
                
                if (TryGetFromCache<T>(assetKey, out var cachedAsset))
                {
                    operation.OnComplete(_ => ResourceSystem.Instance.RemoveFromZeroRefKeys(assetKey));
                    operation.SetResult(cachedAsset);
                    returnKey = assetKey;
                    return operation;
                }
                
                resultKey = assetKey;
                operation.OnComplete(_ => ResourceSystem.Instance.RemoveFromZeroRefKeys(assetKey));
                _fetchLoader.LoadAsset(assetKey, operation);
            }
            catch (Exception exception)
            {
                operation.SetError(exception);
            }

            returnKey = resultKey;
            return operation;
        }

        /// <summary>
        /// Attempts to get an asset synchronously from the cache.
        /// Returns false if the asset isn't cached or any error occurs.
        /// </summary>
        /// <typeparam name="T">The type of asset to retrieve</typeparam>
        /// <param name="assetKeyOrigin">Key to identify the asset</param>
        /// <param name="resultAsset">The retrieved asset if successful</param>
        /// <param name="returnKey"></param>
        public bool TryGetAssetSafety<T>(object assetKeyOrigin, out T resultAsset, out AssetKey returnKey) where T : Object
        {
            resultAsset = null;
            
            try
            {
                if (!TryResolveAssetKey(assetKeyOrigin, typeof(T), out var assetKey))
                {
                    returnKey = default;
                    return false;
                }

                ValidateGameObjectRequest<T>();

                returnKey = default;
                if (!TryGetFromCache(assetKey, out resultAsset))
                {
                    return false;
                }
                
                returnKey = assetKey;
                ResourceSystem.Instance.RemoveFromZeroRefKeys(assetKey);
                return true;

            }
            catch (Exception exception)
            {
                LogGetSafetyError(exception);
                returnKey = default;
                return false;
            }
        }

        #region Helper Methods

        private static void LogGetSafetyError(Exception exception) =>
            AddressableLog.Error($"Exception with GetSafety: {exception.Message}");
        
        #endregion
    }
}