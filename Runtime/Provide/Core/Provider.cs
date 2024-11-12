
namespace ActionFit.Framework.Addressable
{
    internal partial class Provider : NeedResourceSystemRegistry, IProvider
    {
        private readonly IFetchLoader _fetchLoader;
        
        public Provider(ResourceSystemRegistry resourceSystemRegistry, IFetchLoader fetchLoader) 
            : base(resourceSystemRegistry)
        {
            _fetchLoader = fetchLoader;
        }

        public void Dispose() { }
    }
}