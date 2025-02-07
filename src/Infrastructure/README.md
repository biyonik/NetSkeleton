# Infrastructure Layer

Bu katman, uygulamanın teknik altyapısını ve dış servislerle olan entegrasyonlarını içerir.

## 📁 Katman Yapısı

```
Infrastructure/
├── Persistence/                # Veritabanı işlemleri
│   ├── Configurations/         # Entity type configurations
│   ├── Context/               # DbContext sınıfları
│   ├── Interceptors/          # SaveChanges interceptors
│   ├── Repositories/          # Repository implementasyonları
│   ├── Extensions/            # Infrastructure extension methods
│   └── Migrations/            # EF Core migrations
├── Services/                   # Dış servis implementasyonları
│   ├── Storage/               # Dosya depolama servisleri
│   ├── Email/                 # Email servisleri
│   ├── SMS/                   # SMS servisleri
│   └── Payment/               # Ödeme servisleri
├── Identity/                   # Kimlik doğrulama/yetkilendirme
├── Caching/                    # Cache mekanizması
├── MessageBrokers/            # Message queue sistemleri
├── Logging/                   # Loglama altyapısı
└── BackgroundJobs/           # Arkaplan işleri
```

## 🔑 Temel Bileşenler

### Database Context
- PostgreSQL integration
- Multi-tenancy desteği
- Audit logging
- Soft delete
- Global query filters
- Change tracking

### Base Repository
- Generic repository implementasyonu
- Specification pattern desteği
- Unit of work pattern
- Async operasyonlar
- Read/Write repository ayrımı

### Entity Configurations
- Fluent API kullanımı
- İlişki tanımlamaları
- Index tanımlamaları
- Unique constraint'ler
- Default değerler

### Interceptors
- SaveChanges öncesi/sonrası işlemler
- Audit log kaydı
- Domain event dispatch
- Soft delete yönetimi
- Concurrency kontrol

## 🛠️ Teknik Özellikler

### Persistence
- Entity Framework Core
- Repository Pattern
- Unit of Work Pattern
- Database Migrations
- Data Seeding

### Caching
- Distributed Cache
- In-Memory Cache
- Cache Invalidation
- Cache Tags

### Message Brokers
- RabbitMQ Integration
- Message Publishing
- Message Consuming
- Dead Letter Queues

### Background Jobs
- Hangfire Integration
- Recurring Jobs
- Delayed Jobs
- Job Monitoring

## 📦 Bağımlılıklar

```xml
<ItemGroup>
    <!-- Entity Framework Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    
    <!-- Caching -->
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    
    <!-- Message Broker -->
    <PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
    
    <!-- Background Jobs -->
    <PackageReference Include="Hangfire.Core" Version="1.8.7" />
    
    <!-- Logging -->
    <PackageReference Include="Serilog" Version="3.1.1" />
</ItemGroup>
```

## ⚙️ Konfigürasyon

### Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=YourDb;Username=user;Password=pass"
  }
}
```

### Redis
```json
{
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "YourApp:"
  }
}
```

### RabbitMQ
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## 🔄 Development Workflow

1. Entity Configuration:
    - DbContext'e entity'yi ekle
    - Configuration sınıfını oluştur
    - İlişkileri tanımla
    - Migration oluştur

2. Repository Implementation:
    - Base repository'den türet
    - Custom metodları ekle
    - Unit test yaz

3. Background Job:
    - Job sınıfını oluştur
    - Schedule'ı tanımla
    - Error handling ekle
    - Monitoring kur

4. Service Implementation:
    - Interface'i tanımla
    - Concrete sınıfı oluştur
    - Exception handling ekle
    - Logging ekle

## 📝 Best Practices

1. Repository Pattern:
    - Generic repository kullan
    - Specification pattern uygula
    - Async metodları tercih et
    - Read/Write ayrımı yap

2. Database:
    - Index stratejisi belirle
    - Soft delete uygula
    - Audit log tut
    - Concurrency yönet

3. Caching:
    - Cache key stratejisi belirle
    - TTL değerleri tanımla
    - Cache invalidation planla
    - Circuit breaker uygula

4. Message Broker:
    - Queue isimlendirme standardı
    - Retry mekanizması
    - Dead letter handling
    - Message persistence