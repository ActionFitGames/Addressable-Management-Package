

using System;
using System.Threading.Tasks;

namespace ActionFit.Framework.Addressable
{
    internal class Process : NeedResourceSystemRegistry, IProcess
    {
        #region Fields

        private readonly IInitializer _initializer;

        private TaskCompletionSource<bool> _initializeCompleteSource;

        // Readonly Fields
        public IPrepareLoader PrepareLoader { get; }
        public IFetchLoader FetchLoader { get; }

        #endregion

        internal Process(ResourceSystemRegistry resourceSystemRegistry) : base(resourceSystemRegistry)
        {
            _initializer = new Initializer(resourceSystemRegistry);
            _initializeCompleteSource = new TaskCompletionSource<bool>();

            PrepareLoader = new PrepareLoader(resourceSystemRegistry);
            FetchLoader = new FetchLoader(resourceSystemRegistry);

            InitializeSequence();
        }

        #region Private

        private void InitializeSequence()
        {
            InitializeInternalAsync().ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    return;
                }
                
                AddressableLog.Error($"Initialization failed with error: {task.Exception}");
                _initializeCompleteSource.SetResult(false);
            });
        }

        private async Task InitializeInternalAsync()
        {
            try
            {
                await _initializer.InitializeSystem(OnStrategyInternalAction);
            }
            catch (Exception exception)
            {
                AddressableLog.Error($"Failed to initialize addressable system: {exception.Message}");
                OnStrategyInternalAction(false);
                throw;
            }
        }

        private void OnStrategyInternalAction(bool isSucceeded)
        {
            ResourceSystem.IsActivateInitialize = isSucceeded;

            if (isSucceeded)
            {
                AddressableLog.Debug("Addressable system initialize succeeded.");
                _initializeCompleteSource.SetResult(true);
            }
            else
            {
                AddressableLog.Error("Addressable system initialize failed.");
                _initializeCompleteSource.SetResult(false);
            }
        }

        #endregion

        public static async Task WaitUntilInit()
        {
            await Task.Run(async () =>
            {
                while (!ResourceSystem.IsActivateInitialize)
                {
                    await Task.Yield();
                }
            });
        }

        public void Dispose()
        {
            _initializeCompleteSource?.TrySetCanceled();
            _initializeCompleteSource = null;
        }
    }
}
