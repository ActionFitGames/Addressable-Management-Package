
using System;
using System.Collections.Generic;

namespace ActionFit.Framework.Addressable
{
    internal class ResourceSystemRegistry : IDisposable
    {
        #region Fields

        internal readonly HashSet<AssetKey> CachedAssetKeys;
        internal readonly Dictionary<AssetKey, AssetReferenceSt> LoadedAssetMap;

        #endregion

        internal ResourceSystemRegistry()
        {
            CachedAssetKeys = new HashSet<AssetKey>();
            LoadedAssetMap = new Dictionary<AssetKey, AssetReferenceSt>();
        }

        public void Dispose()
        {
            CachedAssetKeys.Clear();
            LoadedAssetMap.Clear();
        }
    }
}
