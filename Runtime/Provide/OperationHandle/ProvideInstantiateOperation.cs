
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    internal class ProvideInstantiateOperation : IProvideInstantiateOperation
    {
        private readonly TaskCompletionSource<GameObject> _completionSource = new();
        private readonly List<Action<GameObject>> _completeCallbacks = new();
        private readonly List<Action<Exception>> _errorCallbacks = new();
        
        public bool IsCompleted { get; private set; }
        public bool HasError { get; private set; }
        public Task<GameObject> Task => _completionSource.Task;

        public IProvideInstantiateOperation OnComplete(Action<GameObject> callback)
        {
            _completeCallbacks.Add(callback);
            if (IsCompleted && Task.IsCompletedSuccessfully)
            {
                callback(Task.Result);
            }
            return this;
        }

        public IProvideInstantiateOperation OnError(Action<Exception> callback)
        {
            _errorCallbacks.Add(callback);
            if (HasError && Task.IsFaulted)
            {
                callback(Task.Exception?.InnerException ?? Task.Exception);
            }
            return this;
        }
        
        internal void SetResult(GameObject instance)
        {
            IsCompleted = true;
            foreach (var callback in _completeCallbacks)
            {
                callback(instance);
            }
            _completionSource.SetResult(instance);
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