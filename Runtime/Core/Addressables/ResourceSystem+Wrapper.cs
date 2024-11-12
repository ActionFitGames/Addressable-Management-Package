
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class ResourceSystem
    {
        internal static IPrepareLoadOperation PrepareLoad(object labelReference)
        {
            return Instance._process.PrepareLoader.PrepareLoadAsset(labelReference);
        }
        
        internal static IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
        {
            return Instance._provider.GetAsset<T>(assetKeyOrigin);
        }

        internal static bool TryGetAssetSafety<T>(object assetKeyOrigin, out T result) where T : Object
        {
            return Instance._provider.TryGetAssetSafety(assetKeyOrigin, out result);
        }
    }
}