# SDLC Automation Tool

A .NET 8 console application using System.CommandLine and Spectre.Console.

## Structure

- **Commands/BaseCommand.cs**: Abstract base class for all commands, providing common functionality and helper methods for console output using Spectre.Console
- **Commands/RootCommand.cs**: Root command for the application
- **Program.cs**: Entry point that sets up and invokes the command-line parser

## Building and Running

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Show help
dotnet run -- --help

# Show version
dotnet run -- --version
```

## Adding New Commands

To add a new command, create a class that extends `BaseCommand` and add it as a subcommand to the `RootCommand` in its constructor.

Example:
```csharp
public class MyCommand : BaseCommand
{
    public MyCommand() : base("mycommand", "Description of my command")
    {
        // Add options and arguments here
        this.SetHandler(() => Execute());
    }

    private void Execute()
    {
        WriteSuccess("Command executed successfully!");
    }
}
```

Then add it to RootCommand:
```csharp
public RootCommand() : base("sdlc", "SDLC Automation Tool")
{
    AddCommand(new MyCommand());
}
```
