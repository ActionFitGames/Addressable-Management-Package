
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal class AssetCustomRef
    {
        internal ushort ReferenceCount;
        internal bool IsPrepare;
        internal Object Asset;
        internal List<AssetCustomRefDependencies> Dependencies;

        internal static AssetCustomRef Empty => default;
    }

    internal class AssetCustomRefDependencies
    {
        public readonly System.Type Type;
        public readonly Object Asset;
        public readonly string SubObjectName; // 서브오브젝트 이름 (디버깅용)

        public AssetCustomRefDependencies(System.Type type, Object asset)
        {
            Type = type;
            Asset = asset;
            SubObjectName = asset?.name ?? string.Empty;
        }
    }
}