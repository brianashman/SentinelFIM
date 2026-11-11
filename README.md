# SentinelFIM

SentinelFIM is a cross-platform File Integrity Monitoring (FIM) platform built with C# and .NET that detects unauthorized file changes through cryptographic integrity verification and real-time monitoring.

By establishing a trusted baseline and continuously validating file integrity, SentinelFIM can identify file modifications, deletions, and other unexpected changes that may indicate tampering, malware activity, or unauthorized system changes.

Unlike traditional single-purpose monitoring tools, SentinelFIM is designed as an extensible platform. Monitoring, hashing, reporting, and client functionality are separated into independent components, making it possible to introduce new capabilities without modifying existing monitoring logic.

## Features

### Integrity Monitoring

- Cryptographic integrity verification
- Baseline generation
- Baseline comparison
- File modification detection
- File creation detection
- File deletion detection
- Real-time filesystem monitoring
- Multiple independent monitoring sessions

#### Current Implementations

- SHA-256 hashing provider
- FileSystemWatcher monitoring provider

### Reporting

- Structured monitoring output
- Extensible reporting architecture
- Planned JSON, TXT, YAML, XML, and CSV exports

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
> `docs/Excalidraw/SentinelFIM.excalidraw` and can be opened with
> [Excalidraw](https://excalidraw.com/).

> The editable source for the Mermaid diagram can be found in
> `docs/Mermaid/SentinelFIMLayeredArchitecture.mmd` and can be opened with
> [Mermaid Live Editor](https://mermaid.live/).

SentinelFIM follows a layered architecture that separates integrity monitoring, hashing, reporting, and client functionality into independent components.

The core engine remains independent of specific implementations, while hashing, monitoring, and reporting functionality are provided through interchangeable providers. This allows new algorithms, monitoring technologies, export formats, and user interfaces to be introduced without modifying existing business logic.

The architecture is designed to support long-term growth while maintaining a clear separation between application logic, infrastructure concerns, and presentation layers.

> A more detailed architectural overview can be found in `docs/architecture-overview.md`

## Current Scope

SentinelFIM currently focuses on scanning and generation baselines for files and directories

### Supported

- Baseline generation
- Integrity monitoring
- Folder & File Based baseline generation
- Recursive directory scanning

### Planned


- Real-time change detection
- Multiple independent monitoring sessions
- Additional hashing providers
- Additional monitoring providers
- Additional export providers
- GUI-based monitor management

## Technical Highlights

- C#
- .NET
- Layered Architecture
- Dependency Injection
- Provider-Based Design
- Runtime Provider Resolution
- Event-Driven Monitoring
- Concurrent Processing
- Cross-Platform Development

## Status

**Version:** 0.1.0

SentinelFIM is an active security engineering project focused on file integrity monitoring, tamper detection, and extensible monitoring architectures.

## Development Notes

The architecture, application logic, and first-party source code for this project were designed and implemented manually.

AI was used as a learning tool for conceptual explanations, architecture discussions, debugging assistance, and documentation support.

## License

MIT License
