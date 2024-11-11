
namespace ActionFit.Framework.Addressable
{
    internal static class AddressableLog
    {
        #region Const Fields
        
        private const string ColorInfo = "#00FF00";
        private const string ColorWarning = "#FFFF00";
        private const string ColorError = "#FF0000";
        
        #endregion

        internal static void Debug(string message)
        {
#if Addressable_Debug_Symbol
            UnityEngine.Debug.Log(FormatMessage(message, ColorInfo));
#endif
        }
        
        internal static void Warning(string message)
        {
#if Addressable_Debug_Symbol
            UnityEngine.Debug.LogWarning(FormatMessage(message, ColorWarning));
#endif
        }
        
        internal static void Error(string message)
        {
#if Addressable_Debug_Symbol
            UnityEngine.Debug.LogError(FormatMessage(message, ColorError));
#endif
        }
        
        private static string FormatMessage(string message, string colorCode)
        {
            return $"<color={colorCode}>{Define.DefaultMessage}</color> {message}";
        }
    }
}