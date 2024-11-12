
using System;
using System.Collections.Generic;

namespace ActionFit.Framework.Addressable
{
    internal class ResourceSystemRegistry : IDisposable
    {
        #region Fields

        internal readonly HashSet<AssetKey> CachedAssetKeys;
        
        internal readonly Dictionary<AssetKey, AssetCustomRef> LoadedAssetMap;

        #endregion

        internal ResourceSystemRegistry()
        {
            CachedAssetKeys = new HashSet<AssetKey>();
            LoadedAssetMap = new Dictionary<AssetKey, AssetCustomRef>();
        }

        public void Dispose()
        {
            CachedAssetKeys.Clear();
            LoadedAssetMap.Clear();
        }
        
        #region Getter

        internal bool TryGetAssetKey(AssetKey accessKey, out AssetKey resultKey)
        {
            return CachedAssetKeys.TryGetValue(accessKey, out resultKey);
        }

        #endregion
    }
}
