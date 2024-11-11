
namespace ActionFit.Framework.Addressable
{
    public static class Resourcer
    {
        public static IPrepareLoadOperation LoadPrepare(object labelReference)
        {
            return ResourceSystem.PrepareLoad(labelReference);
        }
    }
}