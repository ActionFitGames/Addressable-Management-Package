
using System;

namespace ActionFit.Framework.Addressable
{
    internal interface IPrepareLoader : IDisposable
    {
        IPrepareLoadOperation PrepareLoadAsset(object labelReference);
    }
}