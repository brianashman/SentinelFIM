# SentinelFIM

SentinelFIM is a cross-platform File Integrity Monitoring (FIM) platform built with C# and .NET that detects unauthorized file changes through cryptographic verification and real-time monitoring.

The platform establishes a trusted baseline for monitored files and continuously validates their integrity, allowing it to detect modifications, deletions, and other unexpected changes that may indicate tampering, malware activity, or unauthorized access.

Built around a modular architecture, SentinelFIM separates monitoring, hashing, reporting, and client functionality into independent components, making it easy to introduce new providers and interfaces without changing existing core functionality.

## Features

- Real-time file monitoring
- Cryptographic integrity verification
- Baseline generation and comparison
- File creation, modification, and deletion detection
- Multiple independent monitoring sessions
- Extensible provider architecture
- Cross-platform support

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

SentinelFIM follows a layered architecture that isolates security monitoring logic from infrastructure and user-facing applications. The core engine defines the contracts and workflows that drive integrity monitoring, while hashing, monitoring, reporting, and client implementations remain interchangeable.

This approach allows new functionality to be added without modifying existing monitoring logic and enables multiple clients to share the same monitoring engine.

> A more detailed architectural overview can be found in `docs/architecture-overview.md`

## Current Capabilities

- SHA-256 hashing
- FileSystemWatcher monitoring
- Single-file baseline generation
- Real-time integrity monitoring
- Multiple monitoring sessions

## Planned

- Directory monitoring
- Recursive scanning
- Additional hashing providers
- Additional monitoring providers
- Export providers
- Desktop GUI

## Technology

- C#
- .NET
- Dependency Injection
- Layered Architecture
- Provider-Based Design
- Event-Driven Monitoring
- Concurrent Processing

## Status

**Version:** 0.1.0

SentinelFIM is an active security engineering project focused on file integrity monitoring, tamper detection, and extensible monitoring systems.

## License

MIT License
