using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    internal partial class Provider
    {
        #region Instantiate Public APIs
        
        public IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Transform parent = null)
            => InstantiateWithParams(assetKeyOrigin, parent);

        public IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Vector3 position, Quaternion rotation, Transform parent = null)
            => InstantiateWithParams(assetKeyOrigin, parent, position, rotation);
        
        #endregion
        
        #region Internal Implementation

        private IProvideInstantiateOperation InstantiateWithParams(object assetKeyOrigin, Transform parent = null, Vector3? position = null, Quaternion? rotation = null)
        {
            var operation = new ProvideInstantiateOperation();
            
            try
            {
                if (!TryResolveAssetKey(assetKeyOrigin, typeof(GameObject), out var assetKey))
                {
                    operation.SetError(CreateKeyTypeError(assetKeyOrigin));
                    return operation;
                }

                InstantiatePrefab(assetKey, operation, new InstantiateParams(parent, position, rotation));
            }
            catch (Exception exception)
            {
                operation.SetError(exception);
            }

            return operation;
        }

        private void InstantiatePrefab(AssetKey assetKey, ProvideInstantiateOperation operation, InstantiateParams parameters)
        {
            if (TryGetFromCache<GameObject>(assetKey, out var prefab))
            {
                CreateAndSetupInstance(prefab, assetKey, operation, parameters);
                return;
            }

            LoadAndInstantiate(assetKey, operation, parameters);
        }

        private void LoadAndInstantiate(AssetKey assetKey, ProvideInstantiateOperation operation, InstantiateParams parameters)
        {
            var loadOperation = new ProvideLoadOperation<GameObject>();
            _fetchLoader.LoadAsset(assetKey, loadOperation);
            
            loadOperation
                .OnComplete(prefab => CreateAndSetupInstance(prefab, assetKey, operation, parameters))
                .OnError(operation.SetError);
        }

        private void CreateAndSetupInstance(GameObject prefab, AssetKey assetKey, ProvideInstantiateOperation operation, InstantiateParams parameters)
        {
            try
            {
                var instance = CreateInstance(prefab, parameters);
                SetupInstanceTracker(instance, assetKey);
                operation.SetResult(instance);
            }
            catch (Exception exception)
            {
                operation.SetError(exception);
            }
        }

        private static GameObject CreateInstance(GameObject prefab, InstantiateParams parameters)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            return parameters.HasTransform
                ? Object.Instantiate(prefab, parameters.Position, parameters.Rotation, parameters.Parent)
                : Object.Instantiate(prefab, parameters.Parent);
        }

        private void SetupInstanceTracker(GameObject instance, AssetKey assetKey)
        {
            var tracker = instance.AddComponent<MonoBehaviorTracker>();
            tracker.Key = assetKey;
            tracker.OnDestroyed += HandleInstanceDestroyed;
        }

        private void HandleInstanceDestroyed(MonoBehaviorTracker tracker)
        {
            try
            {
                if (tracker != null && tracker.Key.IsValid)
                {
                    ResourceSystem.Instance.ReleaseAsset(tracker.Key);
                }
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Failed to release instance asset: {exception.Message}");
            }
            finally
            {
                if (tracker != null)
                {
                    tracker.OnDestroyed -= HandleInstanceDestroyed;
                }
            }
        }

        #endregion

        #region Helper Structure

        private readonly struct InstantiateParams
        {
            public readonly Transform Parent;
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly bool HasTransform;

            public InstantiateParams(Transform parent, Vector3? position = null, Quaternion? rotation = null)
            {
                Parent = parent;
                Position = position ?? Vector3.zero;
                Rotation = rotation ?? Quaternion.identity;
                HasTransform = position.HasValue && rotation.HasValue;
            }
        }

        #endregion
    }
}