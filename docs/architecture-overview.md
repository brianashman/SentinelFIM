# Architecture Overview

SentinelFIM follows a layered, contract-driven architecture that separates application concerns into independent projects. This design allows monitoring, hashing, reporting, and client functionality to evolve independently while maintaining a consistent core feature set.

## Project Structure

```text
Presentation
├─ CLI
└─ Future GUI Clients

Infrastructure
├─ Hash Providers
├─ Baseline Storage
├─ Monitoring Implementations
├─ Exporters
└─ File Access

Core
├─ Contracts
├─ Domain Models
└─ Application Logic
```

## Core

The Core project contains the application's business rules and contracts.

Responsibilities include:

- Domain models
- Service contracts (interfaces)
- Baseline comparison logic
- Monitoring abstractions
- Hashing abstractions
- Reporting abstractions

The Core layer contains no platform-specific code and has no dependency on Infrastructure or Presentation.

## Infrastructure

The Infrastructure project contains the concrete implementations required by Core contracts.

Examples include:

- SHA256 hash providers
- Baseline storage implementations
- FileSystemWatcher monitoring implementations
- Export providers
- Filesystem access providers

Infrastructure depends on Core, but Core does not depend on Infrastructure.

## Presentation

The Presentation layer provides user interaction and application hosting.

Current clients:

- Command-line interface (CLI)

Planned clients:

- Desktop GUI
- Mobile GUI

Presentation depends on Core contracts and Infrastructure registrations but contains no business logic.

---

# Service Resolution

SentinelFIM uses an auto-discovery factory pattern to resolve implementations at runtime.

Implementations expose a unique identifier through a contract property.

Example:

```csharp
public interface IHashProvider
{
    string Name { get; }
}
```

```csharp
public sealed class Sha256HashProvider : IHashProvider
{
    public string Name => "SHA256";
}
```

At startup, Dependency Injection registers all available implementations.

Resolvers receive collections of implementations:

```csharp
IEnumerable<IHashProvider>
```

and construct lookup dictionaries that allow services to be selected using user-supplied identifiers:

```csharp
var provider = resolver.Resolve("SHA256");
```

This approach eliminates large switch statements, keeps the architecture extensible, and allows new implementations to be added without modifying existing resolution logic.

---

# Extensibility

Adding a new implementation generally requires:

1. Creating a new contract implementation.
2. Registering the implementation with Dependency Injection.
3. Providing a unique identifier.

Examples include:

- New hash algorithms
- New baseline formats
- New export formats
- New monitoring implementations

This approach supports the Open/Closed Principle by allowing new functionality to be added without modifying existing business logic.

---

# Monitoring Model

SentinelFIM currently focuses on single-file integrity monitoring.

The monitoring workflow is:

```text
File
 ↓
Baseline Generation
 ↓
Baseline Storage
 ↓
Real-Time Monitoring
 ↓
Integrity Verification
 ↓
Reporting
```

Current capabilities include:

- Single-file baseline generation
- Single-file baseline verification
- File modification detection
- File creation detection
- File deletion detection
- Real-time monitoring using FileSystemWatcher

The monitoring engine is designed to support future expansion into directory monitoring, recursive scanning, additional monitoring providers, and alternative event sources while maintaining the same Core contracts and service resolution model.
<br>

Future enhancements may include:

- Directory baseline generation
- Recursive folder monitoring
- Network share monitoring
- Alternative filesystem event providers
- Concurrent monitoring operations
- Distributed monitoring agents

# Design Principles
<br>

Several architectural principles guide SentinelFIM's design:

- Separation of Concerns
- Dependency Inversion
- Open/Closed Principle
- Composition over Inheritance
- Interface-Driven Development
- Dependency Injection
- Extensibility Through Discovery

<br>

By centering the application around contracts defined in Core and resolving implementations through Dependency Injection at runtime, SentinelFIM remains modular, testable, and easily extensible without requiring modifications to existing business logic.
