# Push Notifications - Test Examples

## Ejemplos de Pruebas

### 1. Test de Registro de Token

```csharp
[Fact]
public async Task RegisterDeviceToken_WithValidToken_ReturnsSuccess()
{
    // Arrange
    var request = new RegisterDeviceTokenDto
    {
        Token = "test_token_123",
        Platform = "Android",
        ClienteId = 1
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/Notificaciones/Push/registrar", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<PushNotificationResponseDto>();
    result.Success.Should().BeTrue();
}
```

### 2. Test de Envío a Todos

```csharp
[Fact]
public async Task SendNotification_ToAll_ReturnsCountOfSuccess()
{
    // Arrange
    await RegisterMultipleDevices(); // Helper para registrar varios dispositivos

    var request = new SendPushNotificationDto
    {
        Title = "Test Notification",
        Body = "This is a test",
        TargetType = "all"
    };

    // Act
    var response = await client.PostAsJsonAsync(
        "/api/Notificaciones/Push/enviar", 
        request,
        new AuthenticationHeaderValue("Bearer", validToken)
    );

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<PushNotificationResponseDto>();
    result.Success.Should().BeTrue();
    result.SuccessCount.Should().BeGreaterThan(0);
}
```

### 3. Test de Envío a Empresa

```csharp
[Fact]
public async Task SendNotification_ToEnterprise_ReturnsOnlyEnterpriseDevices()
{
    // Arrange
    var empresaId = Guid.NewGuid();
    await RegisterDevicesForEnterprise(empresaId);

    var request = new SendPushNotificationDto
    {
        Title = "Enterprise Notification",
        Body = "Message for enterprise",
        TargetType = "empresa",
        EmpresaId = empresaId
    };

    // Act
    var response = await client.PostAsJsonAsync(
        "/api/Notificaciones/Push/enviar", 
        request,
        new AuthenticationHeaderValue("Bearer", validToken)
    );

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<PushNotificationResponseDto>();
    result.Success.Should().BeTrue();
}
```

### 4. Test de Desuscripción

```csharp
[Fact]
public async Task Unregister_WithValidToken_DeactivatesToken()
{
    // Arrange
    var token = "test_token_to_unregister";
    await RegisterDeviceToken(token);

    var request = new UnregisterTokenRequest { Token = token };

    // Act
    var response = await client.PostAsJsonAsync("/api/Notificaciones/Push/desuscribir", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<PushNotificationResponseDto>();
    result.Success.Should().BeTrue();

    // Verify token is inactive
    var checkResponse = await client.GetAsync($"/api/Notificaciones/Push/estado/{token}");
    var statusResult = await checkResponse.Content.ReadAsAsync<dynamic>();
    statusResult.isActive.Should().BeFalse();
}
```

### 5. Test de Verificación de Estado

```csharp
[Fact]
public async Task CheckTokenStatus_WithValidToken_ReturnsTokenInfo()
{
    // Arrange
    var token = "test_token_check";
    await RegisterDeviceToken(token);

    // Act
    var response = await client.GetAsync($"/api/Notificaciones/Push/estado/{token}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<dynamic>();
    result.isActive.Should().BeTrue();
    result.platform.Should().NotBeNullOrEmpty();
}
```

### 6. Test de Validación de Errores

```csharp
[Fact]
public async Task RegisterDeviceToken_WithoutToken_ReturnsBadRequest()
{
    // Arrange
    var request = new RegisterDeviceTokenDto { Token = "" };

    // Act
    var response = await client.PostAsJsonAsync("/api/Notificaciones/Push/registrar", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}

[Fact]
public async Task SendNotification_WithoutTitle_ReturnsBadRequest()
{
    // Arrange
    var request = new SendPushNotificationDto
    {
        Title = "",
        Body = "Body without title",
        TargetType = "all"
    };

    // Act
    var response = await client.PostAsJsonAsync(
        "/api/Notificaciones/Push/enviar", 
        request,
        new AuthenticationHeaderValue("Bearer", validToken)
    );

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}

[Fact]
public async Task SendNotification_WithInvalidTargetType_ReturnsBadRequest()
{
    // Arrange
    var request = new SendPushNotificationDto
    {
        Title = "Title",
        Body = "Body",
        TargetType = "invalid_type"
    };

    // Act
    var response = await client.PostAsJsonAsync(
        "/api/Notificaciones/Push/enviar", 
        request,
        new AuthenticationHeaderValue("Bearer", validToken)
    );

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

---

## Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Solo tests de push notifications
dotnet test --filter "Category=PushNotifications"

# Con detalle
dotnet test --verbosity detailed

# Con cobertura
dotnet test /p:CollectCoverage=true
```

---

## Integration Tests - Ejemplo Completo

```csharp
public class PushNotificationIntegrationTests : IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PushNotificationIntegrationTests()
    {
        _factory = new TestWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Setup: Registrar varios dispositivos
        for (int i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync(
                "/api/Notificaciones/Push/registrar",
                new RegisterDeviceTokenDto
                {
                    Token = $"test_token_{i}",
                    Platform = "Android",
                    ClienteId = i
                }
            );
        }
    }

    public async Task DisposeAsync()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task CompleteFlow_RegisterAndSend_Works()
    {
        // Register
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/Notificaciones/Push/registrar",
            new RegisterDeviceTokenDto
            {
                Token = "complete_flow_test",
                Platform = "iOS"
            }
        );
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Send
        var sendResponse = await _client.PostAsJsonAsync(
            "/api/Notificaciones/Push/enviar",
            new SendPushNotificationDto
            {
                Title = "Test",
                Body = "Complete flow test",
                TargetType = "all"
            },
            new AuthenticationHeaderValue("Bearer", GetValidToken())
        );
        sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify
        var verifyResponse = await _client.GetAsync("/api/Notificaciones/Push/estado/complete_flow_test");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

---

## Performance Test - Envío Masivo

```csharp
[Fact]
public async Task SendNotification_To1000Devices_CompletesInReasonableTime()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();

    // Register 1000 devices
    for (int i = 0; i < 1000; i++)
    {
        await _client.PostAsJsonAsync(
            "/api/Notificaciones/Push/registrar",
            new RegisterDeviceTokenDto { Token = $"perf_token_{i}" }
        );
    }

    var request = new SendPushNotificationDto
    {
        Title = "Performance Test",
        Body = "Testing 1000 devices",
        TargetType = "all"
    };

    // Act
    var response = await _client.PostAsJsonAsync(
        "/api/Notificaciones/Push/enviar",
        request,
        new AuthenticationHeaderValue("Bearer", validToken)
    );

    stopwatch.Stop();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // 30 segundos máximo
}
```
