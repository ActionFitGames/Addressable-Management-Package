
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal interface IProvider : IDisposable
    {
        #region Provide Asset

        IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin, out AssetKey returnKey) where T : Object;
        bool TryGetAssetSafety<T>(object assetKeyOrigin, out T resultAsset, out AssetKey returnKey) where T : Object;

        #endregion

        #region Instantiate
        
        IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Transform parent = null);
        IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Vector3 position, Quaternion rotation, Transform parent = null);
        
        #endregion
    }
}