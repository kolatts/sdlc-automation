# Copilot Instructions for SDLC Automation

## Project Purpose
This is an example project demonstrating how to build command-line tools in .NET using System.CommandLine and Spectre.Console. It serves as a reference implementation for CLI applications.

## Architecture
- **Framework**: .NET 8
- **CLI Framework**: System.CommandLine (beta)
- **Console UI**: Spectre.Console for formatted output
- **Pattern**: Command-based architecture with a base class providing common functionality

## Key Files
- `SdlcAutomation/Commands/BaseCommand.cs`: Abstract base class for all commands with helper methods (WriteSuccess, WriteError, WriteInfo, WriteWarning)
- `SdlcAutomation/Commands/RootCommand.cs`: Root command definition
- `SdlcAutomation/Program.cs`: Application entry point
- `SdlcAutomation/README.md`: Primary documentation

## Development Guidelines

### Cross-Platform Support
- Assume users may be on Mac or Windows
- Use cross-platform .NET features only
- Test instructions should work on both platforms

### Documentation
- Keep documentation concise
- Centralize documentation in `SdlcAutomation/README.md`
- Avoid redundant documentation in multiple files

### Code Style
- Use C# nullable reference types (enabled in project)
- Use implicit usings (enabled in project)
- Follow existing patterns for command classes
- Use Spectre.Console markup for colored console output

### Adding New Commands
Commands should:
1. Extend `BaseCommand` class
2. Be registered in `RootCommand` constructor
3. Use helper methods (WriteSuccess, WriteError, WriteInfo, WriteWarning) for output
4. Follow the pattern shown in SdlcAutomation/README.md

## Build and Test

```bash
# Build
dotnet build

# Run
dotnet run

# Run with arguments
dotnet run -- --help
```

## Dependencies
- Spectre.Console: For rich console formatting
- System.CommandLine: For command-line parsing (beta version)
