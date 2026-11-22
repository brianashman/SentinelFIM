# Architecture Overview

SentinelFIM follows a layered, contract-driven architecture that separates business logic, infrastructure concerns, and presentation logic into independent projects.

The architecture is designed around extensibility, maintainability, and separation of concerns, allowing monitoring implementations, hashing providers, storage mechanisms, and future presentation layers to evolve independently without requiring modifications to core application logic.

---

# Project Structure

```text
Presentation
├─ CLI
└─ Future GUI Clients

Infrastructure
├─ Hash Providers
├─ Baseline Storage
├─ Monitoring Implementations
└─ File Access

Core
├─ Contracts
├─ Domain Models
└─ Application Logic
```

---

# Core

The Core project contains SentinelFIM's domain models, contracts, and application rules.

Responsibilities include:

- Domain models
- Monitoring abstractions
- Hashing abstractions
- Baseline abstractions
- Monitoring alerts and status models
- Application-level contracts

The Core layer contains no infrastructure-specific code and is independent of Presentation and Infrastructure projects.

This allows business logic to remain stable even as monitoring technologies, storage mechanisms, and user interfaces evolve over time.

---

# Infrastructure

The Infrastructure project contains the concrete implementations required by Core contracts.

Examples include:

- SHA-256 hash providers
- Baseline storage implementations
- Polling-based monitoring implementations
- Filesystem access services

Infrastructure depends on Core abstractions while Core remains completely independent of Infrastructure.

This dependency direction allows implementations to be replaced without affecting application logic.

Potential future implementations include:

- Event-driven monitoring providers
- Hybrid monitoring implementations
- Additional hash providers
- Alternative baseline formats
- Export providers

---

# Presentation

The Presentation layer hosts user-facing clients and application entry points.

Current clients:

- Command-Line Interface (CLI)

Planned clients:

- Desktop GUI

Presentation is responsible for:

- User interaction
- Command execution
- Rendering monitoring alerts
- Rendering monitoring status notifications
- Dependency Injection configuration

Business logic remains within Core and Infrastructure components.

---

# Dependency Injection

SentinelFIM uses Dependency Injection to decouple application logic from infrastructure implementations.

Core components depend on abstractions while Infrastructure provides concrete implementations.

Examples include:

```csharp
IHashProvider
IBaselineStore
IIntegrityMonitor
```

This approach allows implementations to be replaced without modifying consuming code and provides a foundation for future runtime implementation selection.

The architecture favors composition over inheritance and keeps application logic independent from implementation details.

---

# Monitoring Model

SentinelFIM currently uses a baseline-driven monitoring model.

The monitoring workflow is:

```text
Files & Directories
          ↓
  Baseline Generation
          ↓
   Baseline Storage
          ↓
  Continuous Monitoring
          ↓
 Integrity Verification
          ↓
    Alert Generation
```

The baseline acts as the source of truth.

Rather than comparing files against previous scans, monitored files are continuously evaluated against stored baseline data to detect integrity violations.

Current capabilities include:

- Baseline generation
- Baseline persistence
- Baseline loading
- One-time integrity scans
- Continuous integrity monitoring
- Recursive directory scanning
- Hash change detection
- File size change detection
- File deletion detection
- Monitoring status notifications

---

# Extensibility

SentinelFIM is designed to support future expansion through interchangeable implementations and contract-driven development.

Potential future enhancements include:

## Monitoring

- In-memory monitoring
- Event-driven monitoring
- Hybrid monitoring approaches
- Additional monitoring providers
- Runtime monitor selection

## Reporting

- JSON exports
- CSV exports
- XML exports
- Additional export providers

## User Experience

- Desktop GUI
- Enhanced monitoring views
- Alert management improvements

## Quality & Testing

- Expanded automated test coverage
- Native AOT evaluation
- Additional integration testing

New implementations can be introduced without modifying existing business rules by adhering to Core contracts.

---

# Design Principles

Several architectural principles guide SentinelFIM's design:

- Separation of Concerns
- Dependency Inversion
- Open/Closed Principle
- Composition over Inheritance
- Interface-Driven Development
- Dependency Injection
- Extensibility Through Abstraction

The goal is to keep business rules independent from implementation details while allowing the platform to evolve without requiring large-scale refactoring.

---

# Current Architectural Status

SentinelFIM currently focuses on delivering a complete command-line File Integrity Monitoring platform built around baseline comparison and continuous polling-based monitoring.

The current implementation emphasizes correctness, maintainability, and extensibility while establishing architectural foundations for additional monitoring providers, automated testing, reporting capabilities, and future graphical user interfaces.

Future releases will expand monitoring capabilities while preserving the existing architectural boundaries between Core, Infrastructure, and Presentation layers.
