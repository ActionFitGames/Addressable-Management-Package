using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class PrepareLoader
    {
        /// <summary>
        /// Loads resource locations for given label references and creates a mapping between labels and their locations.
        /// Skips already loaded labels and handles exceptions during loading.
        /// </summary>
        private async Task<Dictionary<AssetLabelReference, IList<IResourceLocation>>> LoadResourceLocations(
            List<AssetLabelReference> labelReferences)
        {
            var locationLabelMap = new Dictionary<AssetLabelReference, IList<IResourceLocation>>();

            foreach (var label in labelReferences)
            {
                if (IsLabelAlreadyLoaded(label))
                    continue;

                try
                {
                    await LoadLocationForLabel(label, locationLabelMap);
                }
                catch (Exception exception)
                {
                    LogLocationLoadError(exception);
                    throw;
                }
            }

            return locationLabelMap;
        }

        /// <summary>
        /// Loads resource locations for a single label and updates related tracking collections.
        /// </summary>
        private async Task LoadLocationForLabel(AssetLabelReference label, 
            IDictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);
            var locations = await handle.Task;
            
            if (locations == null || locations.Count == 0)
            {
                AddressableLog.Warning($"Can't find AssetLabelReference '{label.labelString}'");
                return;
            }

            AddressableLog.Debug($"All Count {locations.Count} Resource Location Loaded");
#if Addressable_Debug_Symbol
            foreach (var location in locations)
            {
                AddressableLog.Debug($"Resource Location: {location.PrimaryKey}");
            }
#endif
            InitializeLocationTracking(label, locations, handle, locationLabelMap);
        }

        /// <summary>
        /// Loads and tracks assets from resource locations, monitoring their loading progress.
        /// </summary>
        private async Task LoadAndTrackAssets(Dictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap,
            PrepareLoadOperation operation)
        {
            try
            {
                var loadOperations = CreateLoadOperations(locationLabelMap);
                await TrackLoadProgress(loadOperations, operation);
            }
            catch (Exception exception)
            {
                LogAssetLoadError(exception);
                throw;
            }
        }

        /// <summary>
        /// Creates load operations for all assets in the location map and initializes progress tracking.
        /// </summary>
        private List<LoadOperationInfo> CreateLoadOperations(Dictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            var loadOperations = new List<LoadOperationInfo>();
            InitializeProgressTracker(locationLabelMap);

            foreach (var (label, locations) in locationLabelMap)
            {
                foreach (var location in locations)
                {
                    var loadOperationInfo = CreateLoadOperation(label, location);
                    if (!loadOperationInfo.Equals(default))
                    {
                        loadOperations.Add(loadOperationInfo);
                    }
                }
            }

            return loadOperations;
        }

        /// <summary>
        /// Tracks the loading progress of all operations and updates the operation status.
        /// </summary>
        private async Task TrackLoadProgress(List<LoadOperationInfo> loadOperations, PrepareLoadOperation operation)
        {
            while (!_progressTracker.IsComplete)
            {
                UpdateLoadProgress(loadOperations, operation);
                await Task.Yield();
            }

            CompleteLoadOperation(operation);
            await Task.WhenAll(loadOperations.Select(x => x.Handle.Task));
        }

        #region Helper Methods and Types
        
        private readonly struct LoadOperationInfo
        {
            public readonly AsyncOperationHandle Handle;
            public readonly AssetLabelReference Label;
            public readonly IResourceLocation Location;

            public LoadOperationInfo(AsyncOperationHandle handle, AssetLabelReference label, IResourceLocation location)
            {
                Handle = handle;
                Label = label;
                Location = location;
            }
        }

        private bool IsLabelAlreadyLoaded(AssetLabelReference label)
        {
            if (!LoadedLabelKeyMap.ContainsKey(label))
            {
                return false;
            }

            AddressableLog.Warning($"Ignore, already '{label}' label asset.");
            return true;
        }

        private void InitializeLocationTracking(AssetLabelReference label, IList<IResourceLocation> locations,
            AsyncOperationHandle handle, IDictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            locationLabelMap.TryAdd(label, locations);
            _operationHandles.Add(handle);
            LoadedLabelKeyMap[label] = new HashSet<AssetKey>();
        }

        private void InitializeProgressTracker(Dictionary<AssetLabelReference, IList<IResourceLocation>> locationLabelMap)
        {
            var totalAssetCount = locationLabelMap.Values.Sum(locations => locations.Count);
            _progressTracker.Initialize(totalAssetCount);
        }

        private LoadOperationInfo CreateLoadOperation(AssetLabelReference label, IResourceLocation location)
        {
            if (location is null)
            {
                return default;
            }
            var handle = Addressables.LoadAssetAsync<Object>(location);
            handle.Completed += op => HandleAssetLoadComplete(op, label, location);
            
            _operationHandles.Add(handle);
            
            return new LoadOperationInfo(handle, label, location);
        }

        private void UpdateLoadProgress(List<LoadOperationInfo> loadOperations, PrepareLoadOperation operation)
        {
            var progress = _progressTracker.CalculateProgress(loadOperations.Select(x => x.Handle));
            operation.UpdateProgress(progress);
            ResourceSystemEventHandler.OnProgressPrepareLoadedCallback?.Invoke(progress);
        }

        private void CompleteLoadOperation(PrepareLoadOperation operation)
        {
            operation.UpdateProgress(1f);
            ResourceSystemEventHandler.OnProgressPrepareLoadedCallback?.Invoke(1f);
        }

        private static void LogLocationLoadError(Exception exception)
            => AddressableLog.Error($"Can't loaded labelLocation. {exception.Message}");

        private static void LogAssetLoadError(Exception exception)
            => AddressableLog.Error($"Can't loaded GroupOperation. {exception.Message}");
        
        #endregion
    }
}