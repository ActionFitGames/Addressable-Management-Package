
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    public static class Resource
    {
        public static IPrepareLoadOperation LoadPrepare(object labelReference)
        {
            return ResourceSystem.PrepareLoad(labelReference);
        }

        public static IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
        {
            return ResourceSystem.GetAsset<T>(assetKeyOrigin);
        }
    }
}