using System.Text.Json;
using QueueManagement.SDK.Models.Webhooks;
using QueueManagement.SDK.Webhooks;
using Xunit;

namespace QueueManagement.SDK.Tests;

public class WebhookHelpersTests
{
    [Fact]
    public void ValidateSignature_ReturnsTrue_WhenSignatureIsValid()
    {
        // Arrange
        var payload = "{\"event\":\"ticket.created\",\"data\":{\"id\":\"123\"}}";
        var secret = "test-secret";
        var signature = WebhookHelpers.GenerateSignature(payload, secret);

        // Act
        var result = WebhookHelpers.ValidateSignature(payload, signature, secret);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_WhenSignatureIsInvalid()
    {
        // Arrange
        var payload = "{\"event\":\"ticket.created\",\"data\":{\"id\":\"123\"}}";
        var secret = "test-secret";
        var invalidSignature = "invalid-signature";

        // Act
        var result = WebhookHelpers.ValidateSignature(payload, invalidSignature, secret);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSignature_ReturnsFalse_WhenPayloadIsEmpty()
    {
        // Arrange
        var payload = "";
        var secret = "test-secret";
        var signature = "any-signature";

        // Act
        var result = WebhookHelpers.ValidateSignature(payload, signature, secret);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GenerateSignature_GeneratesConsistentSignature()
    {
        // Arrange
        var payload = "{\"test\":\"data\"}";
        var secret = "my-secret-key";

        // Act
        var signature1 = WebhookHelpers.GenerateSignature(payload, secret);
        var signature2 = WebhookHelpers.GenerateSignature(payload, secret);

        // Assert
        Assert.Equal(signature1, signature2);
    }

    [Fact]
    public void GenerateSignature_ThrowsArgumentException_WhenPayloadIsEmpty()
    {
        // Arrange
        var payload = "";
        var secret = "test-secret";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => WebhookHelpers.GenerateSignature(payload, secret));
    }

    [Fact]
    public void ParseEvent_ParsesEventSuccessfully()
    {
        // Arrange
        var webhookEvent = new WebhookEvent<TestEventData>
        {
            Id = Guid.NewGuid(),
            WebhookId = Guid.NewGuid(),
            Event = "test.event",
            Data = new TestEventData { Value = "test-value" },
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(webhookEvent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var result = WebhookHelpers.ParseEvent<TestEventData>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(webhookEvent.Event, result.Event);
        Assert.Equal(webhookEvent.Data.Value, result.Data.Value);
    }

    [Fact]
    public void ParseEvent_ThrowsInvalidOperationException_WhenPayloadIsInvalid()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => WebhookHelpers.ParseEvent<TestEventData>(invalidJson));
    }

    [Fact]
    public void TryParseEvent_ReturnsTrue_WhenPayloadIsValid()
    {
        // Arrange
        var webhookEvent = new WebhookEvent<TestEventData>
        {
            Event = "test.event",
            Data = new TestEventData { Value = "test-value" }
        };

        var json = JsonSerializer.Serialize(webhookEvent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var result = WebhookHelpers.TryParseEvent<TestEventData>(json, out var parsedEvent);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEvent);
        Assert.Equal(webhookEvent.Event, parsedEvent.Event);
    }

    [Fact]
    public void TryParseEvent_ReturnsFalse_WhenPayloadIsInvalid()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act
        var result = WebhookHelpers.TryParseEvent<TestEventData>(invalidJson, out var parsedEvent);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEvent);
    }

    [Fact]
    public void ValidateWebhookUrl_ReturnsTrue_ForValidHttpsUrl()
    {
        // Arrange
        var url = "https://example.com/webhook";

        // Act
        var result = WebhookHelpers.ValidateWebhookUrl(url);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateWebhookUrl_ReturnsTrue_ForLocalhostHttp()
    {
        // Arrange
        var urls = new[]
        {
            "http://localhost/webhook",
            "http://localhost:8080/webhook",
            "http://127.0.0.1/webhook",
            "http://127.0.0.1:3000/webhook"
        };

        // Act & Assert
        foreach (var url in urls)
        {
            var result = WebhookHelpers.ValidateWebhookUrl(url);
            Assert.True(result, $"Failed for URL: {url}");
        }
    }

    [Fact]
    public void ValidateWebhookUrl_ReturnsFalse_ForInvalidUrl()
    {
        // Arrange
        var invalidUrls = new[]
        {
            "",
            "not-a-url",
            "ftp://example.com/webhook",
            "http://", // Incomplete URL
            "//example.com/webhook" // Missing scheme
        };

        // Act & Assert
        foreach (var url in invalidUrls)
        {
            var result = WebhookHelpers.ValidateWebhookUrl(url);
            Assert.False(result, $"Should have failed for URL: {url}");
        }
    }

    [Fact]
    public void GenerateSecret_GeneratesUniqueSecrets()
    {
        // Act
        var secret1 = WebhookHelpers.GenerateSecret();
        var secret2 = WebhookHelpers.GenerateSecret();

        // Assert
        Assert.NotEqual(secret1, secret2);
        Assert.NotEmpty(secret1);
        Assert.NotEmpty(secret2);
    }

    [Fact]
    public void GenerateSecret_GeneratesCorrectLength()
    {
        // Arrange
        var length = 16;

        // Act
        var secret = WebhookHelpers.GenerateSecret(length);
        var bytes = Convert.FromBase64String(secret);

        // Assert
        Assert.Equal(length, bytes.Length);
    }

    [Fact]
    public void CalculateRetryDelay_WithoutExponentialBackoff_ReturnsConstantDelay()
    {
        // Arrange
        var config = new WebhookRetryConfig
        {
            UseExponentialBackoff = false,
            InitialDelaySeconds = 10
        };

        // Act
        var delay1 = WebhookHelpers.CalculateRetryDelay(1, config);
        var delay2 = WebhookHelpers.CalculateRetryDelay(2, config);
        var delay3 = WebhookHelpers.CalculateRetryDelay(3, config);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(10), delay1);
        Assert.Equal(TimeSpan.FromSeconds(10), delay2);
        Assert.Equal(TimeSpan.FromSeconds(10), delay3);
    }

    [Fact]
    public void CalculateRetryDelay_WithExponentialBackoff_IncreasesDelay()
    {
        // Arrange
        var config = new WebhookRetryConfig
        {
            UseExponentialBackoff = true,
            InitialDelaySeconds = 10,
            MaxDelaySeconds = 100
        };

        // Act
        var delay1 = WebhookHelpers.CalculateRetryDelay(1, config);
        var delay2 = WebhookHelpers.CalculateRetryDelay(2, config);
        var delay3 = WebhookHelpers.CalculateRetryDelay(3, config);

        // Assert
        Assert.True(delay1.TotalSeconds >= 10 && delay1.TotalSeconds <= 11); // 10 + jitter
        Assert.True(delay2.TotalSeconds >= 20 && delay2.TotalSeconds <= 22); // 20 + jitter
        Assert.True(delay3.TotalSeconds >= 40 && delay3.TotalSeconds <= 44); // 40 + jitter
    }

    [Fact]
    public void CalculateRetryDelay_RespectsMaxDelay()
    {
        // Arrange
        var config = new WebhookRetryConfig
        {
            UseExponentialBackoff = true,
            InitialDelaySeconds = 10,
            MaxDelaySeconds = 30
        };

        // Act
        var delay = WebhookHelpers.CalculateRetryDelay(10, config); // Would be 5120 seconds without max

        // Assert
        Assert.True(delay.TotalSeconds <= 33); // Max delay + jitter
    }

    private class TestEventData
    {
        public string Value { get; set; } = string.Empty;
    }
}