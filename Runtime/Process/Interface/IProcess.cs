
using System;

namespace ActionFit.Framework.Addressable
{
    internal interface IProcess : IDisposable
    {
        IPrepareLoader PrepareLoader { get; }
        IFetchLoader FetchLoader { get; }
    }
}