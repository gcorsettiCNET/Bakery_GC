# 🍞 Bakery Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-blue.svg)](https://docs.microsoft.com/en-us/ef/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Un sistema di gestione panetteria moderno sviluppato con **Clean Architecture** e **Design Patterns** enterprise-ready. Questo progetto dimostra l'implementazione di un'architettura scalabile e maintainable utilizzando i principi DDD (Domain-Driven Design) e le migliori pratiche di sviluppo .NET.

## 🎯 **Obiettivo del Progetto**

Questo repository serve come **portfolio dimostrativo** per mostrare:
- Implementazione di **Clean Architecture** in .NET 9
- **Design Patterns** moderni (Repository, Unit of Work, Result Pattern, CQRS)
- **Best Practices** per progetti enterprise-scale
- **Structured Logging** e gestione errori professionale
- **Testability** e **Maintainability** del codice

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
| **🗃️ Repository Pattern** | Astrazione data access con interfacce | Database agnostic, Testabilità |
| **🔄 Unit of Work** | Coordinamento transazioni multiple | Consistenza ACID, Performance |
| **🎯 Result Pattern** | Gestione errori senza eccezioni | Error handling esplicito, Performance |
| **📦 Dependency Injection** | IoC container per loose coupling | Testabilità, Flessibilità |
| **🏗️ Rich Domain Models** | Business logic nelle entities | Encapsulation, Reusability |

## 🚀 **Getting Started**

### **Prerequisiti**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (opzionale - usa InMemory per default)

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

4. **Esecuzione dell'applicazione:**
```bash
dotnet run --project Bakery_GC
```

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
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9** - ORM con Code-First approach

### **Design Patterns & Architecture**
- **MediatR** - CQRS implementation (in development)
- **FluentValidation** - Validation pipeline
- **AutoMapper** - Object-to-object mapping (planned)

### **Database & Storage**
- **SQL Server** - Production database
- **InMemory Database** - Development/Testing
- **Entity Framework Migrations** - Schema management

### **Testing & Quality**
- **Structured Logging** - Microsoft.Extensions.Logging
- **Result Pattern** - Custom implementation per error handling
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

## 📖 **Esempi di Codice**

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
- [x] Clean Architecture setup
- [x] Repository Pattern + Unit of Work
- [x] Result Pattern per error handling
- [x] Domain Entities con business logic
- [x] Structured logging
- [x] Dependency Injection setup
- [x] InMemory database per testing
- [x] Seed data automatico

### **🔄 In Sviluppo**
- [ ] **CQRS con MediatR** - Separazione Commands/Queries
- [ ] **FluentValidation** - Pipeline di validazione
- [ ] **AutoMapper** - DTOs mapping
- [ ] **Authentication & Authorization**

### **📋 Pianificato**
- [ ] **Serilog** per advanced logging
- [ ] **Unit Tests** completi
- [ ] **Integration Tests**
- [ ] **Docker** containerization
- [ ] **Azure** deployment
- [ ] **Swagger/OpenAPI** documentation
- [ ] **Performance monitoring**

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
- **Microsoft** per .NET ecosystem
- **Community .NET** per best practices e patterns

---

> 💡 **Nota per Recruiters/Tech Leaders**: Questo progetto dimostra competenze in architetture enterprise, design patterns, e best practices per progetti scalabili. È stato sviluppato come showcase di skills tecniche avanzate in ambiente .NET.

## 📚 **Risorse Aggiuntive**

- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Application Architecture Guides](https://docs.microsoft.com/en-us/dotnet/architecture/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)