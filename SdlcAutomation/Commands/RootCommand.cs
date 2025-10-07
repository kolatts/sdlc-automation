using System.CommandLine;

namespace SdlcAutomation.Commands;

public class RootCommand : Command
{
    public RootCommand() : base("sdlc", "SDLC Automation Tool")
    {
        AddCommand(new SettingsCommand());
        AddCommand(new OrganizationCommand());
        AddCommand(new AzureDevOpsCommand());
        AddCommand(new JiraCommand());
    }
}
