
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader
    {
        public IPrepareLoadOperation PrepareLoadAsset(object labelReference)
        {
            var operation = new PrepareLoadOperation();
            
            PrepareLoadAssetInternal(labelReference, operation).ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    operation.SetError(task.Exception.InnerException ?? task.Exception);
                }
            });
            
            return operation;
        }

        #region Private Methods

        private async Task PrepareLoadAssetInternal(object labelReference, PrepareLoadOperation operation)
        {
            var labelReferences = ValidateLabelReference(labelReference);
            if (labelReferences == null)
            {
                return;
            }
            
            await Process.WaitUntilInit();
            
            var locationLabelMap = await LoadResourceLocations(labelReferences);
            if (locationLabelMap == null || !locationLabelMap.Any())
            {
                operation.SetComplete();
                ReleaseAllHandles();
                return;
            }

            try
            {
                await LoadAndTrackAssets(locationLabelMap, operation);
                operation.SetComplete();
                ReleaseAllHandles();
            }
            catch (Exception exception)
            {
                operation.SetError(exception);
                throw;
            }
        }

        private List<AssetLabelReference> ValidateLabelReference(object labelReference)
        {
            return labelReference switch
            {
                AssetLabelReference singleLabel => new List<AssetLabelReference> { singleLabel },
                List<AssetLabelReference> { Count: > 0 } multipleLabels => multipleLabels,
                _ => null
            };
        }

        private void ReleaseAllHandles()
        {
            foreach (var operationHandle in _operationHandles)
            {
                Addressables.Release(operationHandle);
            }
        }

        #endregion
    }
}