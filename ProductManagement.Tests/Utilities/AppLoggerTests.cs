using Microsoft.Extensions.Configuration;
using ProductManagement.Logger;
using Xunit;

namespace ProductManagement.Tests.Utilities
{
    public class AppLoggerTests
    {
        private readonly AppLogger<AppLoggerTests> _logger;

        public AppLoggerTests()
        {
            var configuration = BuildTestConfiguration();
            _logger = new AppLogger<AppLoggerTests>(configuration);
        }

        [Fact]
        public void LogInformation_DoesNotThrow()
        {
            var ex = Record.Exception(() => _logger.LogInformation("Testing info log {0}", 123));
            Assert.Null(ex);
        }

        [Fact]
        public void LogWarning_DoesNotThrow()
        {
            var ex = Record.Exception(() => _logger.LogWarning("Warning occurred"));
            Assert.Null(ex);
        }

        [Fact]
        public void LogError_WithException_DoesNotThrow()
        {
            var ex = Record.Exception(() =>
                _logger.LogError(new InvalidOperationException("boom"), "Something failed"));
            Assert.Null(ex);
        }

        [Fact]
        public void LogError_WithoutException_DoesNotThrow()
        {
            var ex = Record.Exception(() => _logger.LogError("Simple error message"));
            Assert.Null(ex);
        }

        [Fact]
        public void LogDebug_DoesNotThrow()
        {
            var ex = Record.Exception(() => _logger.LogDebug("Debugging test {0}", 456));
            Assert.Null(ex);
        }

        private IConfiguration BuildTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Serilog:MinimumLevel:Default", "Information"},
                {"Serilog:WriteTo:0:Name", "Console"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
    }
}