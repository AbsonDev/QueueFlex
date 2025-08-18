using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using QueueManagement.SDK;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Tickets;
using Xunit;

namespace QueueManagement.SDK.Tests;

public class QueueManagementClientTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly QueueManagementOptions _options;
    private readonly QueueManagementClient _client;

    public QueueManagementClientTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com")
        };
        
        _options = new QueueManagementOptions
        {
            ApiKey = "test-api-key",
            BaseUrl = "https://api.test.com",
            EnableLogging = false,
            MaxRetries = 0 // Disable retries for tests
        };

        _client = new QueueManagementClient(_httpClient, _options, null, false);
    }

    [Fact]
    public async Task HealthCheckAsync_ReturnsTrue_WhenApiIsHealthy()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "{}");

        // Act
        var result = await _client.HealthCheckAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HealthCheckAsync_ReturnsFalse_WhenApiIsUnhealthy()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.ServiceUnavailable, "{}");

        // Act
        var result = await _client.HealthCheckAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetApiInfoAsync_ReturnsApiInfo_WhenSuccessful()
    {
        // Arrange
        var apiInfo = new ApiInfo
        {
            Version = "1.0.0",
            Environment = "production",
            Status = "healthy",
            ServerTime = DateTime.UtcNow,
            TimeZone = "UTC"
        };

        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(apiInfo, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        // Act
        var result = await _client.GetApiInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.0.0", result.Version);
        Assert.Equal("production", result.Environment);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => new QueueManagementClient(null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenApiKeyIsEmpty()
    {
        // Arrange
        var invalidOptions = new QueueManagementOptions
        {
            ApiKey = "",
            BaseUrl = "https://api.test.com"
        };

        // Assert
        Assert.Throws<ArgumentException>(() => new QueueManagementClient(invalidOptions));
    }

    [Fact]
    public void ServiceClients_AreNotNull_AfterInitialization()
    {
        // Assert
        Assert.NotNull(_client.Tickets);
        Assert.NotNull(_client.Queues);
        Assert.NotNull(_client.Sessions);
        Assert.NotNull(_client.Units);
        Assert.NotNull(_client.Services);
        Assert.NotNull(_client.Users);
        Assert.NotNull(_client.Dashboard);
        Assert.NotNull(_client.Webhooks);
        Assert.NotNull(_client.SignalR);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Arrange
        var client = new QueueManagementClient("test-key");

        // Act & Assert
        var exception = Record.Exception(() => client.Dispose());
        Assert.Null(exception);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    public void Dispose()
    {
        _client?.Dispose();
        _httpClient?.Dispose();
    }
}

public class QueueManagementOptionsTests
{
    [Fact]
    public void Validate_ThrowsArgumentException_WhenApiKeyIsEmpty()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            ApiKey = "",
            BaseUrl = "https://api.test.com"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Validate());
    }

    [Fact]
    public void Validate_ThrowsArgumentException_WhenBaseUrlIsInvalid()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            ApiKey = "test-key",
            BaseUrl = "not-a-url"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Validate());
    }

    [Fact]
    public void Validate_ThrowsArgumentException_WhenTimeoutIsNegative()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.test.com",
            Timeout = TimeSpan.FromSeconds(-1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Validate());
    }

    [Fact]
    public void Validate_DoesNotThrow_WhenOptionsAreValid()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.test.com",
            Timeout = TimeSpan.FromSeconds(30),
            MaxRetries = 3
        };

        // Act & Assert
        var exception = Record.Exception(() => options.Validate());
        Assert.Null(exception);
    }

    [Fact]
    public void GetSignalRUrl_ReturnsCorrectUrl_WhenSignalRUrlIsNotSet()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            BaseUrl = "https://api.test.com"
        };

        // Act
        var signalRUrl = options.GetSignalRUrl();

        // Assert
        Assert.Equal("https://api.test.com/hubs/queue", signalRUrl);
    }

    [Fact]
    public void GetSignalRUrl_ReturnsCustomUrl_WhenSignalRUrlIsSet()
    {
        // Arrange
        var options = new QueueManagementOptions
        {
            BaseUrl = "https://api.test.com",
            SignalRUrl = "https://signalr.test.com/hub"
        };

        // Act
        var signalRUrl = options.GetSignalRUrl();

        // Assert
        Assert.Equal("https://signalr.test.com/hub", signalRUrl);
    }
}