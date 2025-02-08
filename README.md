# Modern .NET 8 Enterprise Application Framework

Bu proje, modern .NET 8 tabanlı kurumsal uygulamalar için gelişmiş bir framework sunmaktadır. Domain Driven Design, CQRS, ve Event-Driven Architecture prensiplerini temel alarak oluşturulmuştur.

## 🏗️ Mimari Yapı

Bu framework aşağıdaki mimari prensipleri benimser:
- Domain Driven Design (DDD)
- Command Query Responsibility Segregation (CQRS)
- Object Oriented Programming (OOP)
- Aspect Oriented Programming (AOP)
- Event-Driven Architecture
- Module Monolithic Architecture
- Repository Pattern
- Unit of Work Pattern
- Strategy Pattern
- Factory Pattern

## 📁 Proje Yapısı

```
src/
├── Core/
│   ├── Domain/           # Domain entities, value objects, domain events
│   └── Application/      # Application logic, interfaces, DTOs
├── Infrastructure/
│   ├── Persistence/     # Database context, migrations, repositories
│   └── Infrastructure/  # Cross-cutting concerns, external services
│       ├── Cache/       # Caching strategies (Redis, Memory, File)
│       ├── MessageBroker/ # Message broker strategies (RabbitMQ, InMemory)
│       ├── Email/       # Email strategies (SMTP, File)
│       ├── Storage/     # Storage strategies (Local, Azure, AWS)
│       ├── BackgroundJobs/ # Background job service (Hangfire)
│       └── Logging/     # Logging service (Serilog)
├── Presentation/
│   ├── API/            # REST API endpoints
│   └── Web/            # MVC Web application
└── tests/
    ├── Unit/
    ├── Integration/
    └── Functional/
```

## 🚀 Infrastructure Servisleri

### Cache Service
Üç farklı strateji ile cache yönetimi:
```csharp
// Servis registrasyonu
services.AddCacheServices(options =>
{
    options.Strategy = CacheStrategy.Redis; // veya Memory, File
    options.Redis = new RedisSettings
    {
        ConnectionString = "localhost:6379",
        InstanceName = "MyApp:"
    };
});

// Kullanım
public class ProductService
{
    private readonly ICacheManager _cacheManager;

    public async Task<Product> GetProductAsync(string id)
    {
        var cacheKey = $"product:{id}";
        var product = await _cacheManager.GetAsync<Product>(cacheKey);
        
        if (product == null)
        {
            product = await _dbContext.Products.FindAsync(id);
            await _cacheManager.SetAsync(cacheKey, product, TimeSpan.FromHours(1));
        }
        
        return product;
    }
}
```

### Message Broker Service
Event-driven mimari için mesajlaşma altyapısı:
```csharp
// Servis registrasyonu
services.AddMessageBroker(options =>
{
    options.Strategy = MessageBrokerStrategy.RabbitMQ;
    options.RabbitMQ = new RabbitMQSettings
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
});

// Event publish etme
await _messageBroker.PublishAsync("orders", new OrderCreatedEvent(orderId));

// Event subscribe etme
await _messageBroker.SubscribeAsync<OrderCreatedEvent>("orders", 
    async (@event) => await ProcessOrderAsync(@event));
```

### Email Service
SMTP ve dosya bazlı email gönderimi:
```csharp
// Servis registrasyonu
services.AddEmailServices(options =>
{
    options.Strategy = EmailStrategy.Smtp;
    options.Smtp = new SmtpSettings
    {
        Host = "smtp.gmail.com",
        Port = 587,
        UserName = "your@email.com",
        Password = "your-password"
    };
});

// Email gönderme
var email = new EmailMessage
{
    To = { "user@example.com" },
    Subject = "Welcome!",
    Body = "Welcome to our platform."
};

await _emailManager.SendAsync(email);

// Template ile email gönderme
await _emailManager.SendTemplatedEmailAsync(
    "WelcomeEmail",
    new { UserName = "John" },
    email);
```

### Storage Service
Dosya depolama servisi:
```csharp
// Servis registrasyonu
services.AddStorageServices(options =>
{
    options.Strategy = StorageStrategy.AzureBlob;
    options.AzureBlob = new AzureBlobStorageSettings
    {
        ConnectionString = "your-connection-string",
        ContainerName = "files"
    };
});

// Dosya yükleme
var fileModel = await _storageManager.UploadAsync(
    stream,
    "document.pdf",
    "documents");

// Dosya indirme
var fileStream = await _storageManager.DownloadAsync(fileId);
```

### Background Job Service
Hangfire ile arkaplan işleri:
```csharp
// Servis registrasyonu
services.AddBackgroundJobs(configuration);
app.UseBackgroundJobDashboard(configuration);

// Job ekleme
_jobService.Enqueue<IEmailJob>(
    job => job.SendWelcomeEmailAsync(user.Email));

// Zamanlanmış job
_jobService.Schedule<IEmailJob>(
    job => job.SendReminderEmail(user.Email),
    TimeSpan.FromDays(7));

// Recurring job
_jobService.RecurringJob<ICleanupJob>(
    "daily-cleanup",
    job => job.CleanupAsync(),
    "0 0 * * *");
```

### Logging Service
Serilog tabanlı loglama:
```csharp
// Servis registrasyonu
services.AddCustomLogging(configuration);
app.UseRequestResponseLogging();

// appsettings.json
{
  "Logging": {
    "MinimumLevel": "Information",
    "WriteToConsole": true,
    "File": {
      "Path": "logs/log-.txt",
      "RollingInterval": "Day"
    },
    "Enricher": {
      "ApplicationName": "MyApp",
      "Environment": "Production"
    }
  }
}

// Kullanım
_logger.LogInformation(
    "User {UserId} performed {Action}",
    user.Id,
    "login");
```

## 🔧 Kurulum

1. Repository'yi klonlayın
```bash
git clone <repository-url>
```

2. Gerekli paketleri yükleyin
```bash
dotnet restore
```

3. Veritabanını oluşturun
```bash
cd src/Infrastructure/Persistence
dotnet ef database update
```

4. Uygulamayı çalıştırın
```bash
cd src/Presentation/API
dotnet run
```

## 📦 NuGet Paketleri

### Core
- MediatR
- FluentValidation

### Infrastructure
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- StackExchange.Redis
- RabbitMQ.Client
- MailKit
- Azure.Storage.Blobs
- AWSSDK.S3
- Hangfire
- Serilog

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🆘 Destek

Sorularınız için Issues bölümünü kullanabilir veya [email@domain.com] adresinden iletişime geçebilirsiniz.