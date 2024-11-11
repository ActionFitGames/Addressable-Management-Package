
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    internal struct AssetReferenceSt
    {
        internal ushort ReferenceCount;
        internal bool IsPrepare;
        internal Object Asset;
    }
}