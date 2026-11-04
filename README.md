# SentinelFIM

SentinelFIM is a cross-platform File Integrity Monitoring (FIM) platform built with C# and .NET that detects unauthorized file changes through cryptographic integrity verification and real-time filesystem monitoring.

Designed using a layered, contract-driven architecture, SentinelFIM separates monitoring, hashing, reporting, and client functionality into independent components, allowing new monitoring providers, hashing algorithms, reporting formats, and user interfaces to be added with minimal changes to existing code.

The project serves as both a practical security tool and a software engineering project focused on maintainable architecture, dependency injection, extensibility, concurrent processing, and cross-platform application development.

## Features

### Integrity Monitoring

- Cryptographic integrity verification
- Baseline generation
- Baseline comparison
- File modification detection
- File creation detection
- File deletion detection
- Real-time filesystem monitoring
- Extensible hashing and monitoring provider architecture

#### Current Implementations

- SHA256 hashing provider
- FileSystemWatcher monitoring provider

### Reporting

- Planned JSON, TXT, YAML, XML, and CSV export support
- Structured monitoring output
- Extensible export architecture

### Platform Support

- Command-line interface (CLI)
- Planned desktop GUI support
- Cross-platform design

## Architecture

<table>
<tr>
<td align="center">
<img src="docs/Excalidraw/architecture.svg" width="500">
<br>
<b>Core Layout</b>
</td>

<td align="center">
<img src="docs/Mermaid/SentinelFIMLayeredArchitecture.svg" width="500">
<br>
<b>Solution Architecture</b>
</td>
</tr>
</table>

> The editable source for the Excalidraw diagram can be found in
> `docs/Excalidraw/SentinelFIM.excalidraw` and can be opened using
> [Excalidraw](https://excalidraw.com/).

> The editable source for the Mermaid diagram can be found in
> `docs/Mermaid/SentinelFIMLayeredArchitecture.mmd` and can be opened using
> [Mermaid Live Editor](https://mermaid.live/).

SentinelFIM follows a shared, layered, contract-driven architecture that separates monitoring, hashing, reporting, storage, and client functionality.

The monitoring engine is designed around event-driven principles, allowing multiple interfaces to consume the same core services while maintaining a consistent monitoring experience across platforms.

Current releases include SHA256 hashing and FileSystemWatcher monitoring providers. However, the platform is designed around provider abstractions that allow alternative implementations to be introduced without modifying existing Core business logic.

Current areas of focus include:

- Real-time monitoring
- Multiple independent monitoring sessions
- Extensible reporting
- Provider-based architecture
- Cross-platform client support

> A more detailed architectural overview can be found in `docs/architecture-overview.md`

## Current Scope

SentinelFIM currently focuses on monitoring individual files.

### Supported

- Single-file baseline generation
- Single-file integrity monitoring
- Real-time change detection
- Multiple independent monitoring sessions

### Planned

- Directory monitoring
- Recursive directory scanning
- Folder-based baseline generation
- Additional hashing providers
- Additional monitoring providers
- Additional export providers
- GUI-based monitor management

## Technical Highlights

- C#
- .NET
- Layered Architecture
- Contract-Driven Design
- Dependency Injection
- Runtime Provider Resolution
- Event-Driven Design
- Provider-Based Extensibility
- Concurrent Processing
- Parallelized File Scanning
- Cryptographic Integrity Verification
- Cross-Platform Application Development

## Status

**Version:** 0.1.0

SentinelFIM is currently under active development as a long-term security engineering project focused on filesystem monitoring, software architecture, extensibility, performance, and cross-platform application design.

Current releases focus on single-file integrity monitoring, while directory monitoring, recursive scanning, additional providers, and advanced monitoring capabilities are planned for future versions.

## License

MIT License
