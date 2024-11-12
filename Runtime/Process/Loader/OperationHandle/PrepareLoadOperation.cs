
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionFit.Framework.Addressable
{
    public class PrepareLoadOperation : IPrepareLoadOperation
    {
        #region Fields

        private readonly TaskCompletionSource<bool> _completionSource = new();
        private readonly List<Action<float>> _progressCallbacks = new();
        private readonly List<Action> _completeCallbacks = new();
        private readonly List<Action<Exception>> _errorCallbacks = new();
        
        public bool IsCompleted { get; private set; }
        public bool HasError { get; private set; }
        public float Progress { get; private set; }
        public Task Task => _completionSource.Task;

        #endregion

        public IPrepareLoadOperation OnProgress(Action<float> callback)
        {
            _progressCallbacks.Add(callback);
            if (Progress > 0)
            {
                callback(Progress);
            }
            return this;
        }

        public IPrepareLoadOperation OnComplete(Action callback)
        {
            _completeCallbacks.Add(callback);
            if (IsCompleted && Task.IsCompletedSuccessfully)
            {
                callback();
            }
            return this;
        }

        public IPrepareLoadOperation OnError(Action<Exception> callback)
        {
            _errorCallbacks.Add(callback);
            if (HasError && Task.IsFaulted)
            {
                callback(Task.Exception?.InnerException ?? Task.Exception);
            }
            return this;
        }

        #region Internal Methods

        internal void UpdateProgress(float progress)
        {
            Progress = progress;
            foreach (var callback in _progressCallbacks)
            {
                callback(progress);
            }
        }

        internal void SetComplete()
        {
            IsCompleted = true;
            foreach (var callback in _completeCallbacks)
            {
                callback();
            }
            _completionSource.SetResult(true);
        }

        internal void SetError(Exception exception)
        {
            HasError = true;
            foreach (var callback in _errorCallbacks)
            {
                callback(exception);
            }
            _completionSource.SetException(exception);
        }

        #endregion
    }
}