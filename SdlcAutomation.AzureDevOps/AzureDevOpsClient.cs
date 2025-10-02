using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace SdlcAutomation.AzureDevOps;

/// <summary>
/// Client for Azure DevOps operations
/// </summary>
public class AzureDevOpsClient : IAzureDevOpsClient
{
    private readonly VssConnection _connection;
    private readonly WorkItemTrackingHttpClient _witClient;
    private readonly string _project;

    /// <summary>
    /// Creates a new Azure DevOps client using PAT authentication from environment variable
    /// </summary>
    /// <param name="organizationUrl">Azure DevOps organization URL</param>
    /// <param name="project">Project name</param>
    /// <param name="patEnvironmentVariable">Environment variable name containing the PAT (default: AZURE_DEVOPS_PAT)</param>
    public AzureDevOpsClient(string organizationUrl, string project, string patEnvironmentVariable = "AZURE_DEVOPS_PAT")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(project);

        var pat = Environment.GetEnvironmentVariable(patEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidOperationException($"Personal Access Token not found in environment variable '{patEnvironmentVariable}'");
        }

        _project = project;
        var credentials = new VssBasicCredential(string.Empty, pat);
        _connection = new VssConnection(new Uri(organizationUrl), credentials);
        _witClient = _connection.GetClient<WorkItemTrackingHttpClient>();
    }

    /// <summary>
    /// Gets a work item by ID
    /// </summary>
    public async Task<WorkItem> GetWorkItemAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _witClient.GetWorkItemAsync(id, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets work items by IDs
    /// </summary>
    public async Task<IEnumerable<WorkItem>> GetWorkItemsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (!idList.Any())
        {
            return Enumerable.Empty<WorkItem>();
        }

        return await _witClient.GetWorkItemsAsync(idList, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Queries work items by type
    /// </summary>
    public async Task<IEnumerable<WorkItem>> QueryWorkItemsByTypeAsync(string workItemType, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workItemType);

        var wiql = new Wiql
        {
            Query = $"SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = '{_project}' AND [System.WorkItemType] = '{workItemType}'"
        };

        var result = await _witClient.QueryByWiqlAsync(wiql, _project, cancellationToken: cancellationToken);
        
        if (result.WorkItems == null || !result.WorkItems.Any())
        {
            return Enumerable.Empty<WorkItem>();
        }

        var ids = result.WorkItems.Select(wi => wi.Id).ToList();
        return await _witClient.GetWorkItemsAsync(ids, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates a work item
    /// </summary>
    public async Task<WorkItem> UpdateWorkItemAsync(int id, IDictionary<string, object> fields, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fields);

        if (!fields.Any())
        {
            throw new ArgumentException("At least one field must be provided for update", nameof(fields));
        }

        var patchDocument = new JsonPatchDocument();
        foreach (var field in fields)
        {
            patchDocument.Add(new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = $"/fields/{field.Key}",
                Value = field.Value
            });
        }

        return await _witClient.UpdateWorkItemAsync(patchDocument, id, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Disposes the client
    /// </summary>
    public void Dispose()
    {
        _witClient?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
