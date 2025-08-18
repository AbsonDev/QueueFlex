# 🔒 Segurança

## Visão Geral

Este documento descreve as práticas, políticas e implementações de segurança do Queue Management System.

## 🔐 Autenticação

### JWT (JSON Web Tokens)

O sistema utiliza JWT Bearer tokens para autenticação.

#### Configuração

```json
{
  "Jwt": {
    "SecretKey": "your-256-bit-secret-key-here",
    "Issuer": "QueueManagement",
    "Audience": "QueueManagement",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

#### Token Structure

```javascript
// Header
{
  "alg": "HS256",
  "typ": "JWT"
}

// Payload
{
  "sub": "user-id",
  "email": "user@example.com",
  "name": "User Name",
  "role": "Manager",
  "tenantId": "tenant-id",
  "iat": 1609459200,
  "exp": 1609462800,
  "iss": "QueueManagement",
  "aud": "QueueManagement"
}
```

#### Implementação

```csharp
public class JwtService : IJwtService
{
    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("TenantId", user.TenantId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Refresh Tokens

Implementação segura de refresh tokens com rotação automática.

```csharp
public class RefreshTokenService
{
    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, User user)
    {
        return user.RefreshToken == token && 
               user.RefreshTokenExpiresAt > DateTime.UtcNow;
    }
}
```

## 🛡️ Autorização

### Role-Based Access Control (RBAC)

#### Roles Disponíveis

| Role | Permissões | Descrição |
|------|------------|-----------|
| **Admin** | Full access | Acesso total ao sistema |
| **Manager** | Manage units, queues, users | Gerenciar unidades e configurações |
| **Operator** | Manage tickets, sessions | Atender clientes |
| **Viewer** | Read-only access | Visualização apenas |

#### Implementação de Políticas

```csharp
// Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", 
        policy => policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireManager", 
        policy => policy.RequireRole("Admin", "Manager"));
    
    options.AddPolicy("RequireOperator", 
        policy => policy.RequireRole("Admin", "Manager", "Operator"));
    
    options.AddPolicy("RequireTenant",
        policy => policy.RequireClaim("TenantId"));
});

// Controller
[Authorize(Policy = "RequireManager")]
public class QueuesController : BaseController
{
    // ...
}
```

### Resource-Based Authorization

```csharp
public class ResourceAuthorizationHandler : 
    AuthorizationHandler<SameAuthorRequirement, Ticket>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameAuthorRequirement requirement,
        Ticket resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (resource.CreatedBy == userId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

## 🔑 Gestão de Senhas

### Hashing com BCrypt

```csharp
public class PasswordService
{
    private const int WorkFactor = 12;
    
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### Política de Senhas

```csharp
public class PasswordValidator
{
    public ValidationResult Validate(string password)
    {
        var errors = new List<string>();
        
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters");
            
        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("Password must contain uppercase letter");
            
        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("Password must contain lowercase letter");
            
        if (!Regex.IsMatch(password, @"[0-9]"))
            errors.Add("Password must contain number");
            
        if (!Regex.IsMatch(password, @"[!@#$%^&*]"))
            errors.Add("Password must contain special character");
            
        return new ValidationResult(errors);
    }
}
```

## 🚦 Rate Limiting

### Configuração Global

```csharp
services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", cancellationToken: token);
    };
});
```

### Rate Limiting por Endpoint

```csharp
[EnableRateLimiting("api")]
public class TicketsController : BaseController
{
    [EnableRateLimiting("strict")]
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        // Limite mais restrito para criação
    }
}
```

## 🔍 Validação de Entrada

### FluentValidation

```csharp
public class CreateTicketValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.QueueId)
            .NotEmpty().WithMessage("Queue is required")
            .Must(BeValidGuid).WithMessage("Invalid queue ID");
            
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(200).WithMessage("Name too long")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Invalid characters in name");
            
        RuleFor(x => x.CustomerDocument)
            .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")
            .When(x => !string.IsNullOrEmpty(x.CustomerDocument))
            .WithMessage("Invalid CPF format");
            
        RuleFor(x => x.CustomerPhone)
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage("Invalid phone format");
    }
    
    private bool BeValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }
}
```

### Sanitização

```csharp
public class InputSanitizer
{
    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        // Remove HTML tags
        var sanitized = Regex.Replace(input, @"<[^>]*>", string.Empty);
        
        // Encode special characters
        sanitized = HttpUtility.HtmlEncode(sanitized);
        
        return sanitized;
    }
    
    public string SanitizeSql(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        // Use parameterized queries instead!
        // This is just for extra safety
        return input.Replace("'", "''")
                   .Replace(";", "")
                   .Replace("--", "")
                   .Replace("/*", "")
                   .Replace("*/", "");
    }
}
```

## 🛡️ Proteção contra Ataques

### SQL Injection

```csharp
// ❌ NUNCA FAÇA ISSO
var query = $"SELECT * FROM tickets WHERE number = '{userInput}'";

// ✅ Use parâmetros
var tickets = await context.Tickets
    .Where(t => t.Number == userInput)
    .ToListAsync();

// ✅ Ou com SQL raw parametrizado
var tickets = await context.Tickets
    .FromSqlRaw("SELECT * FROM tickets WHERE number = {0}", userInput)
    .ToListAsync();
```

### Cross-Site Scripting (XSS)

```csharp
// Middleware para adicionar headers de segurança
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add(
            "Content-Security-Policy",
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';"
        );
        
        await next(context);
    }
}
```

### Cross-Site Request Forgery (CSRF)

```csharp
// Para APIs stateless com JWT, CSRF não é necessário
// Para formulários web tradicionais:
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
```

## 🔐 Criptografia de Dados

### Dados em Trânsito

```csharp
// Forçar HTTPS
app.UseHttpsRedirection();
app.UseHsts();

// Configuração HSTS
services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});
```

### Dados em Repouso

```csharp
public class EncryptionService
{
    private readonly byte[] _key;
    
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        
        var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(
            Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            
        var result = new byte[aes.IV.Length + encrypted.Length];
        Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
        Array.Copy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
        
        return Convert.ToBase64String(result);
    }
    
    public string Decrypt(string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);
        
        using var aes = Aes.Create();
        aes.Key = _key;
        
        var iv = new byte[aes.IV.Length];
        var encrypted = new byte[buffer.Length - iv.Length];
        
        Array.Copy(buffer, 0, iv, 0, iv.Length);
        Array.Copy(buffer, iv.Length, encrypted, 0, encrypted.Length);
        
        aes.IV = iv;
        
        var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
        
        return Encoding.UTF8.GetString(decrypted);
    }
}
```

## 📝 Auditoria e Logging

### Audit Trail

```csharp
public class AuditService
{
    public async Task LogSecurityEventAsync(SecurityEvent evt)
    {
        var audit = new AuditLog
        {
            EventType = evt.Type,
            UserId = evt.UserId,
            TenantId = evt.TenantId,
            IpAddress = evt.IpAddress,
            UserAgent = evt.UserAgent,
            Details = JsonSerializer.Serialize(evt.Details),
            Timestamp = DateTime.UtcNow
        };
        
        await _context.AuditLogs.AddAsync(audit);
        await _context.SaveChangesAsync();
    }
}

// Eventos de segurança para auditar
public enum SecurityEventType
{
    LoginSuccess,
    LoginFailure,
    PasswordChange,
    PasswordReset,
    TokenRefresh,
    UnauthorizedAccess,
    RateLimitExceeded,
    SuspiciousActivity
}
```

### Logging Sensível

```csharp
// Configuração para não logar dados sensíveis
services.AddDbContext<QueueManagementDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    
    #if DEBUG
    options.EnableSensitiveDataLogging();
    #endif
});

// Custom log scrubber
public class SensitiveDataScrubber
{
    private readonly string[] _sensitiveFields = 
    {
        "password", "token", "secret", "key", "authorization"
    };
    
    public string Scrub(string message)
    {
        foreach (var field in _sensitiveFields)
        {
            var pattern = $@"{field}['"":=\s]+([^'""}\s]+)";
            message = Regex.Replace(message, pattern, $"{field}=***REDACTED***", 
                RegexOptions.IgnoreCase);
        }
        
        return message;
    }
}
```

## 🔍 Monitoramento de Segurança

### Detecção de Anomalias

```csharp
public class AnomalyDetectionService
{
    public async Task<bool> DetectBruteForceAsync(string identifier)
    {
        var attempts = await _cache.GetAsync<int>($"login_attempts:{identifier}");
        
        if (attempts > 5)
        {
            await _auditService.LogSecurityEventAsync(new SecurityEvent
            {
                Type = SecurityEventType.SuspiciousActivity,
                Details = new { Reason = "Possible brute force attack", Attempts = attempts }
            });
            
            return true;
        }
        
        return false;
    }
    
    public async Task<bool> DetectUnusualLocationAsync(string userId, string ipAddress)
    {
        var lastLocation = await _cache.GetAsync<string>($"last_location:{userId}");
        var currentLocation = await _geoIpService.GetLocationAsync(ipAddress);
        
        if (lastLocation != null && 
            CalculateDistance(lastLocation, currentLocation) > 1000) // km
        {
            await _auditService.LogSecurityEventAsync(new SecurityEvent
            {
                Type = SecurityEventType.SuspiciousActivity,
                Details = new { Reason = "Login from unusual location" }
            });
            
            return true;
        }
        
        return false;
    }
}
```

## 🚨 Resposta a Incidentes

### Procedimento de Resposta

1. **Detecção**: Alertas automáticos via monitoring
2. **Contenção**: Bloqueio automático de IPs/usuários suspeitos
3. **Investigação**: Análise de logs e audit trail
4. **Remediação**: Correção de vulnerabilidades
5. **Recuperação**: Restauração de serviços
6. **Lições Aprendidas**: Documentação e melhorias

### Contatos de Emergência

```yaml
security_team:
  email: security@queuemanagement.com
  phone: +55 11 9999-9999
  on_call: https://oncall.queuemanagement.com
```

## 📋 Checklist de Segurança

### Desenvolvimento

- [ ] Validação de entrada em todos os endpoints
- [ ] Sanitização de output
- [ ] Uso de prepared statements
- [ ] Autenticação em todos os endpoints privados
- [ ] Autorização baseada em roles
- [ ] Rate limiting configurado
- [ ] Headers de segurança implementados
- [ ] HTTPS forçado
- [ ] Logs sem dados sensíveis

### Deploy

- [ ] Secrets em variáveis de ambiente
- [ ] Certificados SSL válidos
- [ ] Firewall configurado
- [ ] Backup automático ativo
- [ ] Monitoramento de segurança ativo
- [ ] Plano de resposta a incidentes
- [ ] Auditoria de dependências
- [ ] Scanning de vulnerabilidades

### Manutenção

- [ ] Updates de segurança aplicados
- [ ] Revisão periódica de logs
- [ ] Testes de penetração regulares
- [ ] Treinamento de segurança da equipe
- [ ] Revisão de políticas de acesso
- [ ] Rotação de secrets
- [ ] Backup testado regularmente

## 🔗 Recursos

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE/SANS Top 25](https://cwe.mitre.org/top25/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [Security Headers](https://securityheaders.com/)

---

📝 **Última atualização**: Dezembro 2024
🔒 **Classificação**: Confidencial