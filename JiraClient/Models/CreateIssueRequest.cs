using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Request model for creating a JIRA issue
/// </summary>
public class CreateIssueRequest
{
    [JsonPropertyName("fields")]
    public IssueFields Fields { get; set; }

    public CreateIssueRequest(IssueFields fields)
    {
        Fields = fields;
    }
}
