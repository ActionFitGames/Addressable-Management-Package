
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader
    {
        private async Task<Dictionary<AssetLabelReference, IList<IResourceLocation>>> LoadResourceLocations(
            List<AssetLabelReference> labelReferences)
        {
            var locationLabelMap = new Dictionary<AssetLabelReference, IList<IResourceLocation>>();

            foreach (var labelReference in labelReferences)
            {
                if (LoadedLabelKeyMap.ContainsKey(labelReference))
                {
                    AddressableLog.Warning($"Ignore, already '{labelReference}' label asset.");
                    continue;
                }

                try
                {
                    await LoadLocationForLabel(labelReference, locationLabelMap);
                }
                catch (Exception exception)
                {
                    AddressableLog.Error($"Can't loaded labelLocation. {exception.Message}");
                    throw;
                }
            }

            return locationLabelMap;
        }

        private async Task LoadLocationForLabel(
            AssetLabelReference labelReference, 
            IDictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            var handle = Addressables.LoadResourceLocationsAsync(labelReference);
            var labelLocations = await handle.Task;
            
            locationLabelMap.TryAdd(labelReference, labelLocations);
            _operationHandles.Add(handle);
            LoadedLabelKeyMap[labelReference] = new HashSet<AssetKey>();
        }

        private async Task LoadAndTrackAssets(
            Dictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap,
            PrepareLoadOperation operation)
        {
            try
            {
                var loadOperations = CreateLoadOperations(locationLabelMap);
                await TrackLoadProgress(loadOperations, operation);
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Can't loaded GroupOperation. {exception.Message}");
                throw;
            }
        }

        private List<(AsyncOperationHandle Handle, AssetLabelReference Label, IResourceLocation Location)> CreateLoadOperations(
            Dictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            var loadOperations = new List<(AsyncOperationHandle, AssetLabelReference, IResourceLocation)>();
            var totalAssetCount = GetTotalPrepareLoadAssetCount(locationLabelMap.Values);
            _progressTracker.Initialize(totalAssetCount);

            foreach (var (label, locations) in locationLabelMap)
            {
                foreach (var location in locations)
                {
                    if (location.ResourceType == typeof(Texture2D))
                    {
                        continue;
                    }
                    
                    var handle = Addressables.LoadAssetAsync<Object>(location);
                    loadOperations.Add((handle, label, location));
                    _operationHandles.Add(handle);
                    handle.Completed += op => HandleAssetLoadComplete(op, label, location);
                }
            }

            return loadOperations;
        }

        private async Task TrackLoadProgress(
            List<(AsyncOperationHandle Handle, AssetLabelReference Label, IResourceLocation Location)> loadOperations,
            PrepareLoadOperation operation)
        {
            while (!_progressTracker.IsComplete)
            {
                var progress = _progressTracker.CalculateProgress(loadOperations.Select(x => x.Handle));
                operation.UpdateProgress(progress);
                ResourceSystemEventHandler.OnProgressPrepareLoadedCallback?.Invoke(progress);
                await Task.Yield();
            }

            operation.UpdateProgress(1f);
            ResourceSystemEventHandler.OnProgressPrepareLoadedCallback?.Invoke(1f);
            
            await Task.WhenAll(loadOperations.Select(x => x.Handle.Task));
        }

        private int GetTotalPrepareLoadAssetCount(Dictionary<AssetLabelReference, IList<IResourceLocation>>.ValueCollection locationLabelMap)
        {
            return locationLabelMap.Sum(locations => 
                locations.Count(location => location.ResourceType != typeof(Texture2D)));
        }
    }
}