# Synchronous Domain Services

Decision:
Use synchronous file enumeration, hashing, and baseline creation services.

Rationale:
SentinelFIM is primarily a desktop GUI and CLI application where expected concurrency is low. Long-running operations are executed on worker threads by the GUI layer using Task.Run(), allowing the user interface to remain responsive while keeping domain logic simple.

Alternatives Considered:
- Async APIs throughout the application
- IAsyncEnumerable<T>-based path enumeration

Why Rejected:
- Directory enumeration in .NET is fundamentally synchronous.
- Async propagation significantly increased complexity.
- Expected workloads are typically a small number of concurrent operations.
- The scalability benefits did not justify the additional maintenance burden.

Future Considerations:
This decision may be revisited if large-scale concurrent monitoring or distributed scanning is introduced.