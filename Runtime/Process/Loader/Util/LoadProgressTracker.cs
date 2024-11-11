

using System.Collections.Generic;
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActionFit.Framework.Addressable
{
    internal class LoadProgressTracker
    {
        private int _totalCount;
        private int _loadedCount;

        public bool IsComplete => _loadedCount >= _totalCount;

        public void Initialize(int total)
        {
            _totalCount = total;
            _loadedCount = 0;
        }

        public void IncrementLoadedCount()
        {
            _loadedCount++;
        }

        public float CalculateProgress(IEnumerable<AsyncOperationHandle> handles)
        {
            return handles.Sum(h => h.PercentComplete) / _totalCount;
        }
    }
}