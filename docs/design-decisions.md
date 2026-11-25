# Synchronous Domain Services

## Decision

Use synchronous file enumeration, hashing, and baseline creation services.

## Rationale

SentinelFIM is primarily a desktop GUI and CLI application where expected concurrency is low. Long-running operations are executed on worker threads by the GUI layer using `Task.Run()`, allowing the user interface to remain responsive while keeping domain logic simple.

## Alternatives Considered

- Async APIs throughout the application
- `IAsyncEnumerable<T>`-based path enumeration

## Why Rejected

- Directory enumeration in .NET is fundamentally synchronous.
- Async propagation significantly increased complexity.
- Expected workloads are typically a small number of concurrent operations.
- The scalability benefits did not justify the additional maintenance burden.

## Future Considerations

This decision may be revisited if large-scale concurrent monitoring or distributed scanning is introduced.

---

# Testing Philosophy

## Decision

Test observable monitoring behavior rather than private implementation details.

## Rationale

Monitoring implementations may use different persistence sources, baseline loading strategies, and internal state management. These implementation details are considered internal responsibilities of the monitor and may change without affecting external behavior.

Test behavior through the public API of concrete monitors, not through private implementation details.

Examples include:

- Detecting modified files
- Detecting deleted files
- Producing expected alerts
- Respecting cancellation requests
- Polling at the configured interval

## Alternatives Considered

- Testing private methods directly
- Testing internal baseline-loading logic
- Testing internal state-management logic

## Why Rejected

Private implementation details are subject to change and do not represent the monitor's public contract. Tests tied to implementation details become brittle and increase maintenance costs without improving confidence in system behavior.

## Future Considerations

If multiple monitor implementations share common behavioral requirements, contract-style tests may be introduced to verify compliance with `IIntegrityMonitor` across implementations.

---

# Integration Testing Strategy

## Decision

Use real filesystem integration tests for filesystem-related services.

## Rationale

The primary responsibility of these services is interacting with the operating system filesystem. Mocking filesystem behavior would reduce confidence that the application behaves correctly under real-world conditions.

Examples include:

- File enumeration
- Directory traversal
- Baseline generation
- Change detection

Monitoring tests should operate against real baseline files and real filesystem changes whenever practical.

## Alternatives Considered

- Pure mock-based filesystem tests
- Constructing baseline data manually in tests
- Building `ScanResult` objects by hand

## Why Rejected

Pure mock-based tests would primarily validate mock configuration rather than actual filesystem behavior.

Constructing baselines manually duplicates production logic, increases test maintenance, and reduces confidence that the application behaves correctly when processing real baseline files.

## Future Considerations

Additional integration tests may be added for end-to-end monitoring scenarios involving baseline creation, file modification, and alert generation.

---

# Self-Identifying Strategy Implementations

## Decision

Use self-identifying implementations through public string identifiers instead of enum-based selection, type checks, or switch statements.

Examples include:

```csharp
public interface IHashProvider
{
    string Algorithm { get; }
}
```

```csharp
public interface IBaselineReader
{
    string Format { get; }
}
```

```csharp
public interface IBaselineWriter
{
    string Format { get; }
}
```

## Rationale

Implementations are responsible for identifying themselves.

Consumers can discover or select implementations using dependency injection and a common identifier rather than maintaining centralized mapping logic.

This supports extension without requiring modification of existing selection code.

Examples:

```csharp
public class SHA256HashProvider
{
    public string Algorithm => AlgorithmType.SHA256;
}
```

```csharp
public class JsonBaselineReader
{
    public string Format => OutputFormat.Json;
}
```

## Alternatives Considered

- Enums
- Switch statements
- Type checking (`is`, `typeof`)
- Hardcoded mappings between identifiers and implementations

## Why Rejected

- Adding a new implementation requires modifying existing selection code.
- Consumers become coupled to implementation details.
- Switch statements grow as additional formats and algorithms are added.
- Type checking works against polymorphism and becomes difficult to maintain.

## String Constants vs Enums

SentinelFIM uses shared string constants for identifiers rather than enums.

Examples:

```csharp
public static class AlgorithmType
{
    public const string SHA256 = "sha256";
}
```

```csharp
public static class OutputFormat
{
    public const string Json = "json";
}
```

### Rationale

Identifiers originate from user input, configuration, and command-line arguments.

Using string constants:

- Removes repetitive enum-to-string conversions.
- Simplifies command-line parsing.
- Simplifies implementation selection.
- Keeps identifiers consistent across application layers.

## Future Considerations

This pattern may later be used for:

- Additional baseline formats
- Additional hash algorithms
- Monitor implementations
- Alert providers
- Plugin-based component discovery

---

# Interface Segregation for Baseline Persistence

## Decision

Separate baseline reading and baseline writing responsibilities into independent interfaces.

```csharp
public interface IBaselineReader
{
    IEnumerable<ScanResult> LoadBaseline(string path);
}
```

```csharp
public interface IBaselineWriter
{
    BaselineResults SaveBaseline(
        string path,
        string outputPath,
        bool isRecursive,
        CancellationToken cancellationToken);
}
```

Composite consumers may depend on:

```csharp
public interface IBaselineStore : IBaselineReader, IBaselineWriter 
{ 
}
```

## Rationale

Reading and writing baselines have different dependencies and are used by different services.

For example:

- Monitoring requires reading baselines.
- Baseline generation requires writing baselines.

Separating responsibilities reduces unnecessary dependency injection and simplifies testing.

## Alternatives Considered

A single interface:

```csharp
public interface IBaselineStore
{
    IEnumerable<ScanResult> LoadBaseline(...);

    BaselineResults SaveBaseline(...);
}
```

## Why Rejected

Consumers requiring only one capability were forced to depend on unrelated functionality.

This resulted in unnecessary dependencies, additional mocks during testing, and reduced clarity regarding service responsibilities.

## Future Considerations

Additional baseline formats may implement readers and writers independently while still supporting composite store abstractions when both behaviors are required.