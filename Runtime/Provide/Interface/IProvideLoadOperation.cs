
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace ActionFit.Framework.Addressable
{
    public interface IProvideLoadOperation<T> where T : Object
    {
        bool IsCompleted { get; }
        bool HasError { get; }
        
        Task<T> Task { get; }
        
        IProvideLoadOperation<T> OnComplete(Action<T> callback);
        IProvideLoadOperation<T> OnError(Action<Exception> callback);
    }
}