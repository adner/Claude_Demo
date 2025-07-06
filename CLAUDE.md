# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture

This is a Microsoft Dataverse/Power Platform development environment containing:

- **TestPlugin/**: Dataverse plugin development project (.NET Framework 4.6.2)
  - Uses Microsoft.CrmSdk.CoreAssemblies for Dataverse SDK
  - Signed assembly with strong naming (PluginTest.snk)
  - Base plugin class in PluginBase.cs with common functionality
- **TestProject1/**: Unit test project using xUnit and FakeXrmEasy

## Development Commands

### Building
```powershell
# Build entire solution
dotnet.exe build PluginTest.sln
```
### Testing
```powershell
# Run all tests
dotnet.exe test TestProject1/TestProject1.csproj
```

### Git usage
when running git, use the command 'git.exe' and not simply 'git'.

## Key Patterns

- Plugin development follows Microsoft Dataverse SDK patterns with PluginBase inheritance
- Test projects use FakeXrmEasy for mocking Dataverse context
- Service Bus integration uses Azure.Messaging.ServiceBus client
- PowerShell scripts handle Azure authentication and API calls

## Finding bugs in production and fixing them
- If the user asks you to find bugs in production you will follow the instructions below. When doing the stesps, and executing commands - explain briefly but clearly what you are doing. Especially, describe the results of your operations.
- First, you will use the ExecuteFetchXml tool to retrieve records from the plugintracelog table in Dataverse. You will only retrieve the most recent record, disregard the others.
- You will then inspect the messageblock field that contains the trace output from the plugin execution.
- You will then make the necessary modifications to the TestPlugin assembly code to fix the issue.
- You will then create a test in TestProject1 that shows that the identified issue has been resolved.
- You will run the tests, and after running the tests explain clearly what the results were.
- You will then create a git branch with a name that illustrates the fix.
- You will then push this branch to the remote.
- You will then create a pull request by using the github CLI using the command 'gh.exe' for this.
- The pull request is then pending manual review.

## Updating the plugin in production
If the user requests it, you can update the plugin in production after the pull request has been approved and merged to master.

- You will first checkout the master branch and pull changes.
- You will then build the plugin.
- You will then run the command 'bash updatePlugin.sh', which will update the plugin in production.
- You will then inform clearly about which fix was deployed to production.