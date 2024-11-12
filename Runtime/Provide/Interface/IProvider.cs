
using System;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal interface IProvider : IDisposable
    {
        IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object;
        bool TryGetAssetSafety<T>(object assetKeyOrigin, out T resultAsset) where T : Object;
    }
}