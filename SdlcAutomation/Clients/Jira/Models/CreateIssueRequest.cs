namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Request model for creating a JIRA issue
/// </summary>
public class CreateIssueRequest
{
    public IssueFields Fields { get; set; }

    public CreateIssueRequest(IssueFields fields)
    {
        Fields = fields;
    }
}
