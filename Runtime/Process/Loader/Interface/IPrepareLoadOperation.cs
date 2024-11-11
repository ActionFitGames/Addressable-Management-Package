
using System;
using System.Threading.Tasks;

namespace ActionFit.Framework.Addressable
{
    public interface IPrepareLoadOperation
    {
        bool IsCompleted { get; }
        float Progress { get; }

        IPrepareLoadOperation OnProgress(Action<float> onCallback);
        IPrepareLoadOperation OnComplete(Action onCallback);
        IPrepareLoadOperation OnError(Action<Exception> onCallback);
        
        Task Task { get; }
    }
}