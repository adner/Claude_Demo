# Demonstration of Claude/Github
This demo shows how Claude Code can be used in Github Actions.
## Initial setup
Create a test environment:
```powershell
pac admin create --name "AddeTest_20250615" --type Sandbox --region europe --currency USD --language 1033 --domain addetest
```

Create auth for newly created environment:
```powershell
pac auth create --url https://addetest.crm4.dynamics.com/
```
Login using Azure CLI and get a Bearer token.
```powershell
az login

```
Install sample data:
```powershell
.\installSampleData.ps1
```