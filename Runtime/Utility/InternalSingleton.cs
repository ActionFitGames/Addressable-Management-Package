
namespace ActionFit.Framework.Addressable
{
    /// <summary>
    /// Only singleton class use this.
    /// </summary>
    public class LockVolatileClass
    {
        internal static readonly object LockObject = new();
    }
    
    /// <summary>
    /// Provide by 'AddressableSystem' class.
    /// Where only need it.
    /// </summary>
    internal abstract class InternalSingleton<T> : LockVolatileClass where T : ResourceSystem, new()
    {
        private static volatile T _singletonInstance;
        
        /// <summary>
        /// Get the singleton instance of the AddressableSystem
        /// </summary>
        internal static T Instance
        {
            get
            {
                lock(LockObject)
                {
                    if (_singletonInstance != null)
                    {
                        return _singletonInstance;
                    }
                    
                    _singletonInstance = new T();

                    return _singletonInstance;
                }
            }
        }

        /// <summary>
        /// Activate the AddressableSystem, ensuring it is initialize and ready for use.
        /// need AddressableSystem Mono Components.
        /// </summary>
        /// <returns>The singleton instance of the AddressableSystem</returns>
        internal static T Activate()
        {
            AddressableLog.Debug("Activate with Singleton, Generating from 'AddressableSystem'");

            return Instance;
        }
    }
}