
namespace ActionFit.Framework.Addressable
{
    internal abstract class NeedResourceSystemRegistry
    {
        protected readonly ResourceSystemRegistry Registry;

        internal NeedResourceSystemRegistry(ResourceSystemRegistry resourceSystemRegistry)
        {
            Registry = resourceSystemRegistry;
        }
    }
}