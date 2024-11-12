
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace ActionFit.Framework.Addressable
{
    internal interface IPrepareLoader : IDisposable
    {
        Dictionary<AssetLabelReference, HashSet<AssetKey>> LoadedLabelKeyMap { get; }

        void ReleaseLabel(AssetLabelReference releaseLabelRef);
        IPrepareLoadOperation PrepareLoadAsset(object labelReference);
    }
}