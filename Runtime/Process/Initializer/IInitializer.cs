
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace ActionFit.Framework.Addressable
{
    internal interface IInitializer
    {
        Task<IResourceLocator> InitializeSystem(Action<bool> onStrategy, bool autoReleaseHandle = true);
    }
}