# Domain Layer

Bu katman, uygulamanın çekirdek iş mantığını içerir. DDD (Domain-Driven Design) prensiplerine uygun olarak tasarlanmıştır.

## 📁 Katman Yapısı

```
Domain/
├── Common/
│   ├── Abstractions/     # Temel soyut sınıflar
│   │   ├── BaseEntity.cs
│   │   ├── BaseAggregateRoot.cs
│   │   └── BaseValueObject.cs
│   ├── Audit/           # Audit log yapıları
│   │   ├── AuditTrail.cs
│   ├── Events/          # Domain event yapıları
│   │   ├── BaseDomainEvent.cs
│   ├── Exceptions/      # Domain-specific exceptions
│   │   ├── BusinessRuleValidationException.cs
│   │   ├── DomainException.cs
│   │   └── EntityNotFoundException.cs
│   ├── Guards/          # Guard clauses
│   │   └── Guard.cs
│   ├── Interfaces/      # Repository interfaces
│   │   ├── IRepository.cs
│   │   └── IReadRepository.cs
│   └── Specifications/  # Specification pattern
│       └── BaseSpecification.cs
├── Entities/           # Domain entities
├── Events/            # Concrete domain events
│   ├── AuditTrailCreatedDomainEvent.cs
├── ValueObjects/      # Value objects
├── Specifications/    # Concrete specifications
├── Enumerations/     # Smart enums
└── Shared/           # Shared kernel
```

## 🏗️ Temel Yapılar

### Entity Hierarchy
```
BaseEntity<TKey>
    ├── Audit özellikleri
    ├── Domain event desteği
    ├── Soft-delete yeteneği
    └── Versiyon kontrolü
        └── BaseAggregateRoot<TKey>
            └── Aggregate root özellikleri
```

### Guard Clauses
Domain kurallarını korumak için kullanılan yardımcı metotlar:
- AgainstNull
- AgainstNullOrEmpty
- AgainstNegative
- AgainstOutOfRange
- AgainstInvalidEmail
  vb.

### Specification Pattern
Sorgu mantığını enkapsüle eden yapı:
- BaseSpecification
- Composite specifications
- Include expressions
- Sorting
- Paging

### Domain Events
Event-driven mimari için temel yapılar:
- BaseDomainEvent
- Event handlers
- Domain event dispatching

### Audit Trail
Değişiklik takibi için:
- Her entity için audit özellikleri
- Değişiklik logları
- Event-based audit

## 📝 Domain Rules

1. Entity'ler:
   - ID değerleri immutable olmalı
   - İş kuralları entity metodları içinde yer almalı
   - Property'ler için implicit validation
   - Her değişiklik bir domain event üretmeli

2. Value Objects:
   - Tüm property'ler immutable olmalı
   - Factory method pattern ile oluşturulmalı
   - Validation constructor içinde yapılmalı

3. Domain Events:
   - Event adları geçmiş zaman olmalı (Created, Updated, Deleted)
   - Her event için timestamp tutulmalı
   - Event'ler asenkron işlenmeli

4. Repository Interfaces:
   - Generic repository tanımları
   - Read/Write ayrımı (CQS)
   - Specification pattern desteği

## 🔍 Validation Stratejisi

1. Guard Clauses
   - Method parametreleri için
   - Constructor validasyonları için
   - Business rule kontrolleri için

2. Domain Validations
   - Entity seviyesinde validasyonlar
   - Value object validasyonları
   - Cross-entity validasyonlar

## 🚫 Anti-patterns

Kaçınılması gereken durumlar:
- Entity'lerde public setter'lar
- Domain logic'in dışarı sızması
- Anemic domain model
- İş kurallarının entity dışında olması

## 📦 Bağımlılıklar

Domain Layer, projenin en içteki katmanıdır ve minimum dışa bağımlılığa sahiptir:
- MediatR (Domain Events için)

## 🔄 Development Workflow

1. Entity Geliştirme:
   - BaseEntity'den türet
   - Property'leri tanımla
   - Guard clause'ları ekle
   - Domain event'leri tanımla
   - Business method'ları ekle

2. Value Object Geliştirme:
   - BaseValueObject'ten türet
   - Immutable property'ler tanımla
   - Validation ekle
   - Factory method'lar oluştur

3. Specification Geliştirme:
   - BaseSpecification'dan türet
   - Query kriterlerini tanımla
   - Include ifadelerini ekle

4. Event Geliştirme:
   - INotification'ı implemente et
   - Event property'lerini tanımla
   - Handler'ı oluştur