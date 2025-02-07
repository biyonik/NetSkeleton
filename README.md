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
- Specification Pattern

## 📁 Proje Yapısı

```
src/
├── Core/
│   ├── Domain/           # Domain entities, value objects, domain events
│   └── Application/      # Application logic, interfaces, DTOs
├── Infrastructure/
│   ├── Persistence/     # Database context, migrations, repositories
│   └── Infrastructure/  # Cross-cutting concerns, external services
├── Presentation/
│   ├── API/            # REST API endpoints
│   └── Web/            # MVC Web application
└── tests/
    ├── Unit/
    ├── Integration/
    └── Functional/
```

## 🚀 Başlangıç

### Gereksinimler
- .NET 8 SDK
- PostgreSQL (veya tercih edilen başka bir veritabanı)
- Visual Studio 2022 / VS Code / Rider

### Kurulum

1. Repository'yi klonlayın:
```bash
git clone <repository-url>
```

2. Veritabanını oluşturun:
```bash
cd src/Infrastructure/Persistence
dotnet ef database update
```

3. Uygulamayı çalıştırın:
```bash
cd src/Presentation/API
dotnet run
```

## 🛠️ Temel Özellikler

### Domain Layer
- Zengin domain modeli
- Value Objects
- Domain Events
- Custom Exceptions
- Specification Pattern implementasyonu

### Application Layer
- CQRS implementasyonu (MediatR)
- Güçlü validation (FluentValidation)
- Auto Mapping (AutoMapper)
- Application Events

### Infrastructure Layer
- Generic Repository Pattern
- Unit of Work Pattern
- Event Bus implementasyonu
- Caching mekanizması
- Logging altyapısı
- Email service
- File storage service

### Cross-Cutting Concerns
- Exception Handling
- Validation
- Logging
- Caching
- Authentication & Authorization
- Audit Trailing

## 📦 Kullanılan Paketler

### Domain Layer
- MediatR

### Application Layer
- AutoMapper
- FluentValidation
- MediatR
- Microsoft.Extensions.DependencyInjection

### Infrastructure Layer
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.DependencyInjection
- Serilog
- StackExchange.Redis
- RabbitMQ.Client

### API Layer
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- Microsoft.AspNetCore.Mvc.Versioning

## 🔧 Konfigürasyon

`appsettings.json` dosyasında aşağıdaki ayarları yapılandırabilirsiniz:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=YourDb;Username=user;Password=pass"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "EventBus": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## 📋 Best Practices

### Domain Modelleme
- Aggregate Roots tanımlayın
- Entity'ler için behavior-rich modeller kullanın
- Value Objects kullanarak domain logic'i encapsulate edin
- Domain Events ile loosely coupled yapı oluşturun

### Repository Kullanımı
- Generic Repository pattern
- Specification pattern ile query logic'i encapsulate edin
- Unit of Work pattern ile transaction yönetimi

### Validation
- FluentValidation ile domain validation
- Custom validation rules
- Cross-field validation

### Error Handling
- Domain Exceptions
- Global exception handling
- Custom exception middleware

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