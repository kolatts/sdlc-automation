using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using SdlcAutomation.AzureDevOps.Models;

namespace SdlcAutomation.AzureDevOps;

/// <summary>
/// Interface for Azure DevOps client operations
/// </summary>
public interface IAzureDevOpsClient : IDisposable
{
    /// <summary>
    /// Gets a work item by ID
    /// </summary>
    /// <param name="id">The work item ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The work item</returns>
    Task<WorkItem> GetWorkItemAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a work item by ID as a simplified model
    /// </summary>
    /// <param name="id">The work item ID</param>
    /// <param name="options">Options for loading related data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The work item model</returns>
    Task<WorkItemModel> GetWorkItemModelAsync(int id, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work items by IDs
    /// </summary>
    /// <param name="ids">Collection of work item IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of work items</returns>
    Task<IEnumerable<WorkItem>> GetWorkItemsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work items by IDs as simplified models
    /// </summary>
    /// <param name="ids">Collection of work item IDs</param>
    /// <param name="options">Options for loading related data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of work item models</returns>
    Task<IEnumerable<WorkItemModel>> GetWorkItemModelsAsync(IEnumerable<int> ids, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries work items by type
    /// </summary>
    /// <param name="workItemType">Type of work item (Feature, Epic, Story, Task)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of work items</returns>
    Task<IEnumerable<WorkItem>> QueryWorkItemsByTypeAsync(string workItemType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries work items by type
    /// </summary>
    /// <param name="workItemType">Type of work item</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of work items</returns>
    Task<IEnumerable<WorkItem>> QueryWorkItemsByTypeAsync(WorkItemType workItemType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries work items by type as simplified models
    /// </summary>
    /// <param name="workItemType">Type of work item</param>
    /// <param name="options">Options for loading related data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of work item models</returns>
    Task<IEnumerable<WorkItemModel>> QueryWorkItemModelsByTypeAsync(WorkItemType workItemType, WorkItemLoadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a work item
    /// </summary>
    /// <param name="id">The work item ID</param>
    /// <param name="fields">Dictionary of fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated work item</returns>
    Task<WorkItem> UpdateWorkItemAsync(int id, IDictionary<string, object> fields, CancellationToken cancellationToken = default);
}
