using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace ActionFit.Framework.Addressable
{
    public static class Resource
    {
        public static IPrepareLoadOperation LoadPrepare(object labelReference)
        {
            if (labelReference != null)
            {
                return ResourceSystem.PrepareLoad(labelReference);
            }
            
            AddressableLog.Error("Label reference is null");
            return null;
        }

        public static void ReleasePrepare(object labelReferenceOrigin)
        {
            if (labelReferenceOrigin == null)
            {
                AddressableLog.Error("Label reference origin is null");
                return;
            }

            try
            {
                switch (labelReferenceOrigin)
                {
                    case AssetLabelReference labelReference:
                        ResourceSystem.Instance.ReleaseLabel(labelReference);
                        break;
                    case List<AssetLabelReference> labelReferences:
                        foreach (var labelRef in labelReferences.Where(labelRef => labelRef != null))
                        {
                            ResourceSystem.Instance.ReleaseLabel(labelRef);
                        }
                        break;
                    default:
                        AddressableLog.Error($"Unsupported label reference type: {labelReferenceOrigin.GetType()}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                AddressableLog.Error($"Failed to release prepare: {e.Message}");
            }
        }
    }
}