using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using SdlcAutomation.AzureDevOps.Extensions;
using SdlcAutomation.AzureDevOps.Models;

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
    /// Queries work items by type (enum overload)
    /// </summary>
    public async Task<IEnumerable<WorkItem>> QueryWorkItemsByTypeAsync(WorkItemType workItemType, CancellationToken cancellationToken = default)
    {
        return await QueryWorkItemsByTypeAsync(workItemType.GetDescription(), cancellationToken);
    }

    /// <summary>
    /// Gets a work item by ID as a simplified model
    /// </summary>
    public async Task<WorkItemModel> GetWorkItemModelAsync(int id, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= WorkItemLoadOptions.Default;
        var workItem = await _witClient.GetWorkItemAsync(id, expand: WorkItemExpand.Relations, cancellationToken: cancellationToken);
        return await ProjectToModelAsync(workItem, options, cancellationToken);
    }

    /// <summary>
    /// Gets work items by IDs as simplified models
    /// </summary>
    public async Task<IEnumerable<WorkItemModel>> GetWorkItemModelsAsync(IEnumerable<int> ids, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (!idList.Any())
        {
            return Enumerable.Empty<WorkItemModel>();
        }

        options ??= WorkItemLoadOptions.Default;
        var workItems = await _witClient.GetWorkItemsAsync(idList, expand: WorkItemExpand.Relations, cancellationToken: cancellationToken);
        
        var models = new List<WorkItemModel>();
        foreach (var workItem in workItems)
        {
            models.Add(await ProjectToModelAsync(workItem, options, cancellationToken));
        }
        
        return models;
    }

    /// <summary>
    /// Queries work items by type as simplified models
    /// </summary>
    public async Task<IEnumerable<WorkItemModel>> QueryWorkItemModelsByTypeAsync(WorkItemType workItemType, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= WorkItemLoadOptions.Default;
        
        var wiql = new Wiql
        {
            Query = $"SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = '{_project}' AND [System.WorkItemType] = '{workItemType.GetDescription()}'"
        };

        var result = await _witClient.QueryByWiqlAsync(wiql, _project, cancellationToken: cancellationToken);
        
        if (result.WorkItems == null || !result.WorkItems.Any())
        {
            return Enumerable.Empty<WorkItemModel>();
        }

        var ids = result.WorkItems.Select(wi => wi.Id).ToList();
        return await GetWorkItemModelsAsync(ids, options, cancellationToken);
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

    private async Task<WorkItemModel> ProjectToModelAsync(WorkItem workItem, WorkItemLoadOptions options, CancellationToken cancellationToken)
    {
        var model = new WorkItemModel
        {
            Id = workItem.Id ?? 0,
            Title = GetField<string>(workItem, "System.Title"),
            Description = GetField<string>(workItem, "System.Description"),
            AcceptanceCriteria = GetField<string>(workItem, "Microsoft.VSTS.Common.AcceptanceCriteria"),
            AssignedTo = GetField<string>(workItem, "System.AssignedTo"),
            WorkItemType = GetField<string>(workItem, "System.WorkItemType"),
            State = GetField<string>(workItem, "System.State"),
            CreatedDate = GetField<DateTime?>(workItem, "System.CreatedDate"),
            ChangedDate = GetField<DateTime?>(workItem, "System.ChangedDate"),
            ClosedDate = GetField<DateTime?>(workItem, "Microsoft.VSTS.Common.ClosedDate"),
            ResolvedDate = GetField<DateTime?>(workItem, "Microsoft.VSTS.Common.ResolvedDate"),
            ActivatedDate = GetField<DateTime?>(workItem, "Microsoft.VSTS.Common.ActivatedDate"),
            StateChangeDate = GetField<DateTime?>(workItem, "Microsoft.VSTS.Common.StateChangeDate")
        };

        // Load children if requested
        if (options.LoadChildren && workItem.Relations != null)
        {
            var childIds = workItem.Relations
                .Where(r => r.Rel == "System.LinkTypes.Hierarchy-Forward")
                .Select(r => ExtractWorkItemId(r.Url))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (childIds.Any())
            {
                var children = await _witClient.GetWorkItemsAsync(childIds, expand: WorkItemExpand.Relations, cancellationToken: cancellationToken);
                model.Children = new List<WorkItemModel>();
                foreach (var child in children)
                {
                    model.Children.Add(await ProjectToModelAsync(child, WorkItemLoadOptions.Default, cancellationToken));
                }
            }
        }

        // Load parents if requested
        if (options.LoadParents && workItem.Relations != null)
        {
            var parentIds = workItem.Relations
                .Where(r => r.Rel == "System.LinkTypes.Hierarchy-Reverse")
                .Select(r => ExtractWorkItemId(r.Url))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (parentIds.Any())
            {
                var parents = await _witClient.GetWorkItemsAsync(parentIds, expand: WorkItemExpand.Relations, cancellationToken: cancellationToken);
                model.Parents = new List<WorkItemModel>();
                foreach (var parent in parents)
                {
                    model.Parents.Add(await ProjectToModelAsync(parent, WorkItemLoadOptions.Default, cancellationToken));
                }
            }
        }

        // Load commits if requested
        if (options.LoadCommits && workItem.Relations != null)
        {
            var commitLinks = workItem.Relations
                .Where(r => r.Rel == "ArtifactLink" && r.Url?.Contains("Commit") == true)
                .ToList();

            if (commitLinks.Any())
            {
                model.Commits = new List<CommitInfo>();
                foreach (var link in commitLinks)
                {
                    var commit = new CommitInfo
                    {
                        CommitId = ExtractCommitId(link.Url),
                        RemoteUrl = link.Url
                    };
                    
                    if (link.Attributes != null && link.Attributes.ContainsKey("name"))
                    {
                        commit.Comment = link.Attributes["name"]?.ToString();
                    }
                    
                    model.Commits.Add(commit);
                }
            }
        }

        // Load pull requests if requested
        if (options.LoadPullRequests && workItem.Relations != null)
        {
            var prLinks = workItem.Relations
                .Where(r => r.Rel == "ArtifactLink" && r.Url?.Contains("PullRequest") == true)
                .ToList();

            if (prLinks.Any())
            {
                model.PullRequests = new List<PullRequestInfo>();
                foreach (var link in prLinks)
                {
                    var pr = new PullRequestInfo
                    {
                        RemoteUrl = link.Url
                    };
                    
                    if (link.Attributes != null && link.Attributes.ContainsKey("name"))
                    {
                        pr.Title = link.Attributes["name"]?.ToString();
                    }
                    
                    model.PullRequests.Add(pr);
                }
            }
        }

        return model;
    }

    private static T? GetField<T>(WorkItem workItem, string fieldName)
    {
        if (workItem.Fields != null && workItem.Fields.TryGetValue(fieldName, out var value))
        {
            if (value == null)
                return default;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
        return default;
    }

    private static int? ExtractWorkItemId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var lastSegment = url.Split('/').LastOrDefault();
        if (int.TryParse(lastSegment, out var id))
            return id;

        return null;
    }

    private static string? ExtractCommitId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var uri = new Uri(url);
        var segments = uri.Segments;
        
        // Try to find commit ID in URL segments
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Contains("Commit", StringComparison.OrdinalIgnoreCase) && i + 1 < segments.Length)
            {
                return segments[i + 1].TrimEnd('/');
            }
        }

        return null;
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
