# JiraClient.Example

Example console application demonstrating the usage of the JiraClient library.

## Setup

1. Set the required environment variables:

```bash
export JIRA_BASE_URL="https://your-jira-instance.com"
export JIRA_PAT="your-personal-access-token"
```

2. Run the example:

```bash
dotnet run
```

## Usage

The example demonstrates:
- Creating a user story
- Creating a test item
- Creating an issue with custom fields

By default, the code is commented out to prevent accidental issue creation. Uncomment the relevant sections in `Program.cs` and update the project key to create actual JIRA issues.

## Modifying the Example

1. Open `Program.cs`
2. Replace `"PROJ"` with your actual JIRA project key
3. Uncomment the code blocks you want to execute
4. Run with `dotnet run`
