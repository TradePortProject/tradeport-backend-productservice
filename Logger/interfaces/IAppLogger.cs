namespace ProductManagement.Logger.interfaces
{
    public interface IAppLogger<T>
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception? exception, string message, params object[] args);
        void LogError(string message);
        void LogDebug(string message, params object[] args);
    }
}
