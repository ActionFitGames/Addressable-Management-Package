
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    internal interface IFetchLoader
    {
        void LoadAsset<T>(AssetKey assetKey, ProvideLoadOperation<T> operation) where T : Object;
    }
}