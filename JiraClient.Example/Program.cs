using JiraClient;

// Example usage of JiraClient library
// This demonstrates how to create work items in JIRA

Console.WriteLine("JIRA Client Example");
Console.WriteLine("==================\n");

// Check if JIRA_PAT environment variable is set
var jiraBaseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL");
var jiraPat = Environment.GetEnvironmentVariable("JIRA_PAT");

if (string.IsNullOrWhiteSpace(jiraBaseUrl))
{
    Console.WriteLine("ERROR: JIRA_BASE_URL environment variable is not set.");
    Console.WriteLine("Please set it to your JIRA instance URL (e.g., https://jira.example.com)");
    return 1;
}

if (string.IsNullOrWhiteSpace(jiraPat))
{
    Console.WriteLine("ERROR: JIRA_PAT environment variable is not set.");
    Console.WriteLine("Please set it to your JIRA Personal Access Token.");
    return 1;
}

try
{
    Console.WriteLine($"Connecting to JIRA at: {jiraBaseUrl}");
    
    using var client = JiraApiClient.CreateFromEnvironment(jiraBaseUrl);
    
    Console.WriteLine("\nExample 1: Creating a User Story");
    Console.WriteLine("---------------------------------");
    
    // Uncomment the following lines to actually create a user story
    // Replace "PROJ" with your actual project key
    /*
    var storyResponse = await client.CreateUserStoryAsync(
        projectKey: "PROJ",
        summary: "Example User Story from API",
        description: "This is an example user story created via the JiraClient library."
    );
    
    Console.WriteLine($"✓ Created user story: {storyResponse.Key}");
    Console.WriteLine($"  URL: {storyResponse.Self}");
    */
    Console.WriteLine("(Example code commented out - uncomment to create actual issues)");
    
    Console.WriteLine("\nExample 2: Creating a Test Item");
    Console.WriteLine("--------------------------------");
    
    // Uncomment the following lines to actually create a test item
    /*
    var testResponse = await client.CreateTestItemAsync(
        projectKey: "PROJ",
        summary: "Example Test Case from API",
        description: "This is an example test case created via the JiraClient library."
    );
    
    Console.WriteLine($"✓ Created test item: {testResponse.Key}");
    Console.WriteLine($"  URL: {testResponse.Self}");
    */
    Console.WriteLine("(Example code commented out - uncomment to create actual issues)");
    
    Console.WriteLine("\nExample 3: Creating an Issue with Custom Fields");
    Console.WriteLine("-----------------------------------------------");
    
    // Uncomment the following lines to create an issue with custom fields
    /*
    var customFields = new Dictionary<string, object>
    {
        { "customfield_10001", "High" },
        { "labels", new[] { "api-created", "example" } }
    };
    
    var issueResponse = await client.CreateIssueAsync(
        projectKey: "PROJ",
        issueTypeName: "Task",
        summary: "Example Task with Custom Fields",
        description: "This task includes custom fields.",
        additionalFields: customFields
    );
    
    Console.WriteLine($"✓ Created issue: {issueResponse.Key}");
    Console.WriteLine($"  URL: {issueResponse.Self}");
    */
    Console.WriteLine("(Example code commented out - uncomment to create actual issues)");
    
    Console.WriteLine("\n✓ JiraClient library is ready to use!");
    Console.WriteLine("  Uncomment the example code above to create actual JIRA issues.");
    
    return 0;
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"\n✗ Configuration Error: {ex.Message}");
    return 1;
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"\n✗ API Error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ Unexpected Error: {ex.Message}");
    return 1;
}
