
using System;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public static class ResourceSystemEventHandler
    {
        public static Action<Object, IResourceLocation> OnPrepareLoadedCallback;
        public static Action<float> OnProgressPrepareLoadedCallback;
    }
}