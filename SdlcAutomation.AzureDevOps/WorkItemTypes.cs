using System.ComponentModel;

namespace SdlcAutomation.AzureDevOps;

/// <summary>
/// Common Azure DevOps work item types
/// </summary>
public enum WorkItemType
{
    /// <summary>
    /// Feature work item type
    /// </summary>
    [Description("Feature")]
    Feature,

    /// <summary>
    /// Epic work item type
    /// </summary>
    [Description("Epic")]
    Epic,

    /// <summary>
    /// User Story work item type
    /// </summary>
    [Description("User Story")]
    Story,

    /// <summary>
    /// Task work item type
    /// </summary>
    [Description("Task")]
    Task
}
