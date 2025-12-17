# 🍞 Bakery Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![.NET Aspire](https://img.shields.io/badge/.NET%20Aspire-9.0-blueviolet.svg)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-blue.svg)](https://docs.microsoft.com/en-us/ef/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![MediatR](https://img.shields.io/badge/MediatR-CQRS-orange.svg)](https://github.com/jbogard/MediatR)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> 🚀 **Da monolite a microservizi orchestrati con .NET Aspire** - Un percorso evolutivo di architettura software

Un sistema di gestione panetteria moderno sviluppato con **Clean Architecture** e **Design Patterns** enterprise-ready. Questo progetto dimostra l'implementazione di un'architettura scalabile e maintainable utilizzando i principi DDD (Domain-Driven Design) e le migliori pratiche di sviluppo .NET.

## 🎯 **Obiettivo del Progetto**

Questo repository serve come **portfolio dimostrativo** per mostrare:
- Implementazione di **Clean Architecture** in .NET 9
- **Design Patterns** moderni (Repository, Unit of Work, Result Pattern, CQRS)
- **Best Practices** per progetti enterprise-scale
- **Structured Logging** e gestione errori professionale
- **Testability** e **Maintainability** del codice

## 📈 **Evoluzione del Progetto**

Questo progetto rappresenta un percorso evolutivo da applicazione monolitica a architettura distribuita:

```
┌────────────────────────────────────────────────────────────────────────────┐
│                        PERCORSO EVOLUTIVO                                  │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                            │
│  📅 FASE 1: Monolite Web                                                   │
│  ┌─────────────────────┐                                                   │
│  │   Bakery_GC         │  • Razor Pages                                    │
│  │   (Monolite)        │  • Entity Framework                               │
│  │                     │  • Controllers + Views + Data                     │
│  └─────────────────────┘                                                   │
│           │                                                                │
│           ▼                                                                │
│  📅 FASE 2: Clean Architecture                                             │
│  ┌─────────────────────────────────────────────────────┐                   │
│  │  Core  │  Application  │  Infrastructure  │  Web   │                   │
│  │        │               │                  │        │                   │
│  │ Domain │    CQRS +     │   Repositories   │ Razor  │                   │
│  │Entities│   MediatR     │   + DbContext    │ Pages  │                   │
│  └─────────────────────────────────────────────────────┘                   │
│           │                                                                │
│           ▼                                                                │
│  📅 FASE 3: Microservizi + .NET Aspire                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                     Bakery.AppHost                                   │   │
│  │              (Aspire Orchestrator)                                   │   │
│  │  ┌──────────────────┐          ┌──────────────────┐                 │   │
│  │  │ Bakery.OrderSvc  │◄────────►│    Bakery_GC     │                 │   │
│  │  │  (Orders API)    │          │    (Web App)     │                 │   │
│  │  └──────────────────┘          └──────────────────┘                 │   │
│  │           ▲                              ▲                           │   │
│  │           └────────────┬─────────────────┘                           │   │
│  │                        │                                             │   │
│  │              ┌─────────┴──────────┐                                  │   │
│  │              │ ServiceDefaults    │                                  │   │
│  │              │ (Shared Configs)   │                                  │   │
│  │              └────────────────────┘                                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                            │
└────────────────────────────────────────────────────────────────────────────┘
```

### **Motivazioni dell'Evoluzione**

| Fase | Problema Risolto | Pattern/Tecnologia Adottata |
|------|------------------|----------------------------|
| **1 → 2** | Codice accoppiato, difficile da testare | Clean Architecture, CQRS, Repository |
| **2 → 3** | Scalabilità indipendente, team paralleli | Microservizi, .NET Aspire |
| **2 → 3** | Debugging distribuito complesso | OpenTelemetry, Aspire Dashboard |

## 🏗️ **Architettura**

### **Clean Architecture Layers**

Il progetto segue i principi di Clean Architecture con separazione netta delle responsabilità:

```
🌟 DEPENDENCY RULE: Le dipendenze vanno sempre verso l'interno

┌─────────────────────────────────────┐
│        Bakery_GC (Web)              │ ← Presentation Layer
│        • Controllers                │   ASP.NET Core, API, UI
│        • Views/Pages                │   
│        • Program.cs                 │
└─────────────────────────────────────┘
              ↓ dipende da
┌─────────────────────────────────────┐
│     Bakery.Application              │ ← Application Layer  
│        • Use Cases                  │   Business Logic, Services
│        • CQRS Handlers              │
│        • DTOs/ViewModels            │
└─────────────────────────────────────┘
              ↓ dipende da
┌─────────────────────────────────────┐
│     Bakery.Infrastructure           │ ← Infrastructure Layer
│        • DbContext                  │   Data Access, External APIs
│        • Repositories               │
│        • External Services          │
└─────────────────────────────────────┘
              ↓ dipende da
┌─────────────────────────────────────┐
│        Bakery.Core                  │ ← Domain Layer
│        • Entities                   │   Business Rules, Domain Logic
│        • Interfaces                 │   (ZERO dipendenze esterne!)
│        • Domain Services            │
└─────────────────────────────────────┘
```

### **Design Patterns Implementati**

| Pattern | Descrizione | Benefici |
|---------|-------------|----------|
| **🏛️ Clean Architecture** | Separazione layer con dependency inversion | Testabilità, Maintainability, Flessibilità |
| **📨 CQRS + MediatR** | Separazione read/write con handler isolati | Scalabilità, Single Responsibility |
| **🗃️ Repository Pattern** | Astrazione data access con interfacce | Database agnostic, Testabilità |
| **🔄 Unit of Work** | Coordinamento transazioni multiple | Consistenza ACID, Performance |
| **🎯 Result Pattern** | Gestione errori senza eccezioni | Error handling esplicito, Performance |
| **📦 Dependency Injection** | IoC container per loose coupling | Testabilità, Flessibilità |
| **🏗️ Rich Domain Models** | Business logic nelle entities | Encapsulation, Reusability |

### **Struttura della Solution**

```
📁 Bakery_GC.sln
│
├── 🎛️ Bakery.AppHost/              ← .NET Aspire Orchestrator
│   ├── Program.cs                   # Definizione servizi e dipendenze
│   └── appsettings.json
│
├── 📦 Bakery.ServiceDefaults/       ← Shared Aspire Configurations
│   └── Extensions.cs                # OpenTelemetry, Health Checks, Resilience
│
├── 🔵 Bakery.Core/                  ← Domain Layer (ZERO dipendenze)
│   ├── Entities/                    # Product, Customer, Market, Order
│   ├── Interfaces/                  # IRepository<T>, IUnitOfWork
│   └── Common/                      # Result<T>, Enums, Constants
│
├── 🟢 Bakery.Application/           ← Application Layer
│   ├── Commands/                    # CreateProductCommand, etc.
│   ├── Queries/                     # GetAllProductsQuery, etc.
│   ├── Handlers/                    # MediatR Handlers
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Mappings/                    # AutoMapper Profiles
│   └── Behaviors/                   # Pipeline Behaviors (Logging, Validation)
│
├── 🟠 Bakery.Infrastructure/        ← Infrastructure Layer
│   ├── Data/                        # ApplicationDbContext
│   ├── Repositories/                # GenericRepository, ProductRepository
│   ├── Configuration/               # DatabaseConfiguration
│   └── Extensions/                  # ServiceCollectionExtensions
│
├── 🌐 Bakery_GC/                    ← Web Application (Presentation)
│   ├── Controllers/                 # API Controllers, TestController
│   ├── Pages/                       # Razor Pages
│   ├── wwwroot/                     # Static files
│   └── Program.cs                   # DI Configuration, Middleware
│
└── 📦 Bakery.OrderService/          ← Orders Microservice (API)
    ├── Controllers/                 # OrdersController
    └── Program.cs                   # Minimal API setup con Scalar
```

## 🚀 **Getting Started**

### **Prerequisiti**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/install-aspire) (per orchestrazione)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opzionale, per container)

### **Installazione Aspire Workload**

```bash
dotnet workload update
dotnet workload install aspire
```

### **Setup del Progetto**

1. **Clone del repository:**
```bash
git clone https://github.com/gcorsettiCNET/Bakery_GC.git
cd Bakery_GC
```

2. **Restore delle dipendenze:**
```bash
dotnet restore
```

3. **Build del progetto:**
```bash
dotnet build
```

### **Modalità di Esecuzione**

#### **🎛️ Opzione 1: .NET Aspire (Raccomandato)**
Avvia tutti i servizi orchestrati con dashboard Aspire:
```bash
dotnet run --project Bakery.AppHost
```
Apri il browser su `https://localhost:17225` per la **Aspire Dashboard**.

#### **🌐 Opzione 2: Solo Web Application**
Esegui solo l'applicazione web standalone:
```bash
dotnet run --project Bakery_GC
```
L'applicazione si avvierà su `http://localhost:5019`.

#### **📦 Opzione 3: Solo Order Service API**
Esegui solo il microservice ordini:
```bash
dotnet run --project Bakery.OrderService
```
API disponibile con Scalar UI su `/scalar/v1`.

L'applicazione si avvierà su `https://localhost:5019` (o porta simile).

### **Configurazione Database**

#### **InMemory Database (Default - Development)**
Il progetto è configurato per usare un database InMemory per facilità di testing:
- Nessun setup richiesto
- Seed data automatico
- Perfetto per demo e sviluppo

#### **SQL Server (Production)**
Per ambiente production, modifica `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "BakeryContext": "Server=.;Database=BakeryDB;Trusted_Connection=True;"
  }
}
```

Quindi esegui le migrazioni:
```bash
dotnet ef database update --project Bakery.Infrastructure --startup-project Bakery_GC
```

## 🧪 **Testing dell'Architettura**

Il progetto include endpoint di test per dimostrare i pattern implementati:

### **Endpoint Disponibili**

| Endpoint | Descrizione | Pattern Dimostrato |
|----------|-------------|-------------------|
| `GET /api/test/health` | Health check architettura | Dependency Injection, Repository |
| `GET /api/test/products` | Lista prodotti | Repository Pattern, Result Pattern |
| `GET /api/test/customers/vip` | Clienti VIP | Business Logic, Domain Models |
| `POST /api/test/test-transaction` | Test transazioni | Unit of Work, Transaction Management |
| `GET /api/test/products/by-market/{id}` | Prodotti per market | Repository specifici |

### **Esempio di Response**

**Health Check Response:**
```json
{
  "status": "Healthy",
  "architecture": "Clean Architecture with Repository Pattern + Unit of Work",
  "database": "Connected",
  "productsCount": 5,
  "customersCount": 2,
  "designPatterns": [
    "Repository Pattern",
    "Unit of Work",
    "Result Pattern", 
    "Dependency Injection",
    "Clean Architecture"
  ]
}
```

## 🔮 **.NET Aspire Integration**

Il progetto utilizza **.NET Aspire** per l'orchestrazione dei servizi distribuiti:

### **Architettura Aspire**

```
┌─────────────────────────────────────────────────────────────┐
│                    Bakery.AppHost                           │
│              (Aspire Orchestrator)                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────┐    ┌─────────────────────┐        │
│  │  Bakery.OrderService │    │      Bakery_GC      │        │
│  │    (API Service)     │◄───│    (Web Frontend)   │        │
│  │                      │    │                     │        │
│  │  • /api/orders       │    │  • Razor Pages      │        │
│  │  • Scalar Docs       │    │  • MediatR Handlers │        │
│  └─────────────────────┘    └─────────────────────┘        │
│           ▲                           ▲                     │
│           │                           │                     │
│  ┌────────┴───────────────────────────┴────────┐           │
│  │          Bakery.ServiceDefaults              │           │
│  │  • OpenTelemetry Tracing & Metrics          │           │
│  │  • Health Checks                             │           │
│  │  • Service Discovery                         │           │
│  │  • Resilience (Retry, Circuit Breaker)      │           │
│  └──────────────────────────────────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

### **Esecuzione con Aspire**

```bash
# Avvia l'intera soluzione orchestrata
dotnet run --project Bakery.AppHost

# Aspire Dashboard disponibile per:
# • Visualizzazione dei servizi
# • Distributed tracing
# • Logs centralizzati
# • Metriche in tempo reale
```

### **Componenti Aspire**

| Progetto | Ruolo | Funzionalità |
|----------|-------|--------------|
| `Bakery.AppHost` | Orchestrator | Coordina tutti i servizi, gestisce dipendenze |
| `Bakery.OrderService` | API | Gestione ordini, documentazione Scalar |
| `Bakery.ServiceDefaults` | Shared Config | OpenTelemetry, Health Checks, Resilience |
| `Bakery_GC` | Web App | Frontend con Razor Pages, consuma OrderService |

### **Comunicazione tra Servizi**

```csharp
// AppHost - Orchestrazione
var orderApi = builder.AddProject<Projects.Bakery_OrderService>("orderservice");
var webApp = builder.AddProject<Projects.Bakery_GC>("webapp")
    .WithReference(orderApi);  // Dependency injection automatica

// WebApp può chiamare OrderService tramite Service Discovery
// http://orderservice/api/orders (risolto automaticamente)
```

## 📊 **Struttura del Database**

### **Domain Entities**

```
📦 Products (Table Per Hierarchy)
├── Product (base entity)
├── Pizza (ingredients, size, spicy)
├── Bread (type, gluten-free, shelf life)
├── Cake (flavor, occasion, serving size)
└── Pastrie (type, filling, vegan)

👥 People
├── Market (stores/locations)
└── Customer (extends Person)

📋 Orders (future implementation)
├── Order
├── OrderItem  
└── Delivery
```

### **Business Logic Examples**

```csharp
// Rich Domain Models con business logic
var discountedPrice = product.CalculateDiscountedPrice(10); // 10% sconto
var canOrder = product.CanBeOrdered(); // Verifica disponibilità
var vipDiscount = customer.GetVipDiscountPercentage(); // Sconto basato su spesa totale
var isFresh = bread.IsFresh(); // Verifica freschezza basata su shelf life
```

## 🛠️ **Tecnologie Utilizzate**

### **Core Framework**
- **.NET 9** - Latest framework Microsoft
- **ASP.NET Core** - Web framework per Razor Pages e API
- **Entity Framework Core 9** - ORM con Code-First approach

### **.NET Aspire**
- **AppHost** - Orchestrazione dei servizi distribuiti
- **ServiceDefaults** - Configurazione condivisa (OpenTelemetry, Health Checks)
- **Service Discovery** - Risoluzione automatica degli endpoint
- **Resilience** - Retry policies e circuit breaker integrati

### **Design Patterns & Architecture**
- **MediatR** - CQRS pattern per separazione Commands/Queries
- **AutoMapper** - Object-to-object mapping
- **Result Pattern** - Error handling esplicito senza eccezioni

### **Database & Storage**
- **SQLite** - Development database (zero setup)
- **SQL Server** - Production database
- **InMemory Database** - Testing
- **Entity Framework Migrations** - Schema management

### **Observability & Quality**
- **OpenTelemetry** - Distributed tracing e metriche
- **Health Checks** - Monitoraggio stato servizi
- **Structured Logging** - Microsoft.Extensions.Logging
- **Aspire Dashboard** - Visualizzazione centralizzata

## 📖 **Esempi di Codice**

### **CQRS con MediatR**

```csharp
// Query per ottenere prodotti (Read operation)
public class GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>>> { }

// Handler separato per la query
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var result = await _productRepository.GetAllAsync();
        if (!result.IsSuccess) return Result<IEnumerable<ProductDto>>.Failure(result.Error);
        
        return Result<IEnumerable<ProductDto>>.Success(_mapper.Map<IEnumerable<ProductDto>>(result.Value));
    }
}

// Controller pulito che usa MediatR
[ApiController]
public class TestController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
```

### **Repository Pattern Usage**

```csharp
// Controller pulito con dependency injection
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    
    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await _productRepository.GetByIdAsync(id);
        
        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }
        
        return Ok(result.Value);
    }
}
```

### **Result Pattern Implementation**

```csharp
// Gestione errori senza eccezioni
public async Task<Result<Product>> GetProductAsync(Guid id)
{
    try
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
        {
            return Result<Product>.Failure($"Product {id} not found");
        }
        
        return Result<Product>.Success(product);
    }
    catch (Exception ex)
    {
        return Result<Product>.Failure(ex.Message);
    }
}
```

### **Unit of Work Transaction**

```csharp
// Transazione coordinata tra multiple entities
await _unitOfWork.BeginTransactionAsync();
try 
{
    var product = await _unitOfWork.Repository<Product, Guid>().AddAsync(newProduct);
    var customer = await _unitOfWork.Repository<Customer, Guid>().UpdateAsync(existingCustomer);
    
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitTransactionAsync();
}
catch 
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

## 🧪 **Testing Strategy**

### **Architettura Testabile**

L'architettura Clean permette testing efficace a ogni livello:

```csharp
// Unit test del domain (zero dipendenze)
[Test]
public void Product_CalculateDiscountedPrice_ShouldReturnCorrectAmount()
{
    var product = new Product { Price = 100m };
    
    var discountedPrice = product.CalculateDiscountedPrice(10m);
    
    Assert.That(discountedPrice, Is.EqualTo(90m));
}

// Integration test con mock repository
[Test] 
public async Task ProductController_GetProduct_ShouldReturnProduct()
{
    var mockRepo = new Mock<IProductRepository>();
    mockRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Product>.Success(product));
    
    var controller = new ProductController(mockRepo.Object);
    var result = await controller.GetProduct(productId);
    
    Assert.That(result, Is.InstanceOf<OkObjectResult>());
}
```

## 🚧 **Roadmap**

### **✅ Completato**
- [x] Clean Architecture setup con 4 layer
- [x] Repository Pattern + Unit of Work
- [x] Result Pattern per error handling
- [x] CQRS con MediatR (Commands/Queries/Handlers)
- [x] AutoMapper per DTOs mapping
- [x] Domain Entities con business logic (Product, Customer, Market)
- [x] Structured logging con Pipeline Behaviors
- [x] Multi-database support (SQLite, SQL Server, InMemory)
- [x] Seed data automatico con dati realistici italiani
- [x] .NET Aspire integration
- [x] OrderService microservice (API separata)
- [x] OpenTelemetry per distributed tracing
- [x] Health Checks e Resilience patterns

### **🔄 In Sviluppo**
- [ ] **FluentValidation** - Pipeline di validazione
- [ ] **Authentication & Authorization** - JWT/Identity
- [ ] **Order domain completo** - Entità e logica ordini

### **📋 Pianificato**
- [ ] **Unit Tests** completi con xUnit
- [ ] **Integration Tests** con TestContainers
- [ ] **Docker Compose** per deployment locale
- [ ] **Azure Container Apps** deployment
- [ ] **API Gateway** con YARP
- [ ] **Event-Driven Architecture** con message broker

## 🤝 **Contributing**

Questo è un progetto portfolio dimostrativo, ma feedback e suggerimenti sono benvenuti!

1. Fork del progetto
2. Crea feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push branch (`git push origin feature/AmazingFeature`)
5. Apri Pull Request

## 📝 **License**

Questo progetto è rilasciato sotto licenza MIT. Vedi `LICENSE` file per dettagli.

## 👨‍💻 **Autore**

**Giuseppe Corsetti**
- GitHub: [@gcorsettiCNET](https://github.com/gcorsettiCNET)
- LinkedIn: [Giuseppe Corsetti](https://linkedin.com/in/giuseppe-corsetti)

## 🙏 **Riconoscimenti**

- **Robert C. Martin** per Clean Architecture
- **Microsoft** per .NET Aspire e l'ecosistema .NET 9
- **Jimmy Bogard** per MediatR
- **Community .NET** per best practices e patterns

---

> 💡 **Nota per Recruiters/Tech Leaders**: Questo progetto dimostra l'evoluzione da un'applicazione web monolitica a un'architettura distribuita orchestrata con .NET Aspire. Showcase di competenze in Clean Architecture, CQRS, microservizi e observability per progetti enterprise-scale.

## 📚 **Risorse Aggiuntive**

- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [MediatR Wiki](https://github.com/jbogard/MediatR/wiki)
- [.NET Application Architecture Guides](https://docs.microsoft.com/en-us/dotnet/architecture/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)