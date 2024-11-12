
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public class ProvideLoadOperation<T> : IProvideLoadOperation<T> where T : Object
    {
        #region Fields

        private readonly TaskCompletionSource<T> _completionSource = new();
        private readonly List<Action<T>> _completeCallbacks = new();
        private readonly List<Action<Exception>> _errorCallbacks = new();
        
        public bool IsCompleted { get; private set; }
        public bool HasError { get; private set; }
        public Task<T> Task => _completionSource.Task;

        #endregion
        
        public IProvideLoadOperation<T> OnComplete(Action<T> callback)
        {
            _completeCallbacks.Add(callback);
            if (IsCompleted && Task.IsCompletedSuccessfully)
            {
                callback(Task.Result);
            }
            return this;
        }

        public IProvideLoadOperation<T> OnError(Action<Exception> callback)
        {
            _errorCallbacks.Add(callback);
            if (HasError && Task.IsFaulted)
            {
                callback(Task.Exception?.InnerException ?? Task.Exception);
            }
            return this;
        }
        
        internal void SetResult(T result)
        {
            IsCompleted = true;
            foreach (var callback in _completeCallbacks)
            {
                callback(result);
            }
            _completionSource.SetResult(result);
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
    }
}