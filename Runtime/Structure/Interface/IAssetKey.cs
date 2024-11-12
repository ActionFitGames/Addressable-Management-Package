
using System;

namespace ActionFit.Framework.Addressable
{
    internal interface IAssetKey
    {
        string Primary { get; }
        string Runtime { get; }
        string InternalID { get; }
        Type Type { get; }
        bool IsValid { get; }
    }
}