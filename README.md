# SDLC Automation

A collection of tools and libraries for SDLC automation.

## Projects

### SdlcAutomation
Main console application using System.CommandLine and Spectre.Console.

See [SdlcAutomation/README.md](SdlcAutomation/README.md) for more details.

### JiraClient
A .NET 8 client library for interacting with JIRA Data Center REST API.

Features:
- Personal Access Token (PAT) authentication via environment variable
- Create user stories and test-related work items
- No third-party NuGet package dependencies
- Built for JIRA Data Center

See [JiraClient/README.md](JiraClient/README.md) for usage documentation.

### JiraClient.Example
Example console application demonstrating JiraClient usage.

See [JiraClient.Example/README.md](JiraClient.Example/README.md) for more details.

## Building

```bash
# Build all projects
dotnet build SdlcAutomation.sln

# Or build individual projects
dotnet build SdlcAutomation/SdlcAutomation.csproj
dotnet build JiraClient/JiraClient.csproj
dotnet build JiraClient.Example/JiraClient.Example.csproj
```

## License

Apache License 2.0
