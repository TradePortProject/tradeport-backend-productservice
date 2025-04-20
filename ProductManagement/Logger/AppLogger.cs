using ProductManagement.Logger.interfaces;
using Serilog;

namespace ProductManagement.Logger
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly Serilog.ILogger _serilog;

        // Constructor that creates the Serilog logger
        public AppLogger(IConfiguration configuration) 
        {
            _serilog = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Read configuration from appsettings.json
                .CreateLogger()
                .ForContext<T>(); // Add contex
        }

        public void LogInformation(string message, params object[] args)
        {
            _serilog.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _serilog.Warning(message, args);
        }

        public void LogError(Exception? exception, string message, params object[] args)
        {
            _serilog.Error(exception, message, args);
        }

        public void LogError(string message)
        {
            _serilog.Error(message);
        }

        public void LogDebug(string message, params object[] args)
        {
            _serilog.Debug(message, args);
        }
    }
}
