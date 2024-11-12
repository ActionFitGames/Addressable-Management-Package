
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionFit.Framework.Addressable
{
    public interface IProvideInstantiateOperation
    {
        bool IsCompleted { get; }
        bool HasError { get; }
        
        Task<GameObject> Task { get; }
        
        IProvideInstantiateOperation OnComplete(Action<GameObject> callback);
        IProvideInstantiateOperation OnError(Action<Exception> callback);
    }
}