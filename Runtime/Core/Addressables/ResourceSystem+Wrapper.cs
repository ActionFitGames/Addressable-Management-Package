
namespace ActionFit.Framework.Addressable
{
    internal partial class ResourceSystem
    {
        internal static IPrepareLoadOperation PrepareLoad(object labelReference)
        {
            return Instance._process.PrepareLoader.PrepareLoadAsset(labelReference);
        }

        internal static IProvider GetProvider()
        {
            return Instance._provider;
        }
    }
}