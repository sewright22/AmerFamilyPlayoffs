# Azure DevOps MCP Server Setup Guide

This guide will help you set up an Azure DevOps MCP server to automate your deployment pipelines directly from Kiro.

## Option 1: Tiberriver256 Azure DevOps MCP Server (Recommended)

This is a well-maintained community MCP server with extensive features.

### Installation

1. **Clone the repository:**
```bash
git clone https://github.com/Tiberriver256/mcp-server-azure-devops.git
cd mcp-server-azure-devops
npm install
npm run build
```

2. **Add to your MCP configuration:**

Add this to your `~/.kiro/settings/mcp.json`:

```json
{
  "mcpServers": {
    "homelab": {
      "command": "C:\\Program Files\\nodejs\\node.exe",
      "args": ["C:\\Users\\sewri\\repos\\homelab-mcp-server\\dist\\index.js"],
      "env": {},
      "autoApprove": ["execute_command", "get_system_info", "transfer_file"],
      "disabled": false
    },
    "azure-devops": {
      "command": "C:\\Program Files\\nodejs\\node.exe",
      "args": ["C:\\path\\to\\mcp-server-azure-devops\\dist\\index.js"],
      "env": {
        "AZURE_DEVOPS_AUTH_METHOD": "pat",
        "AZURE_DEVOPS_ORG_URL": "https://dev.azure.com/YourOrganization",
        "AZURE_DEVOPS_PAT": "your-personal-access-token",
        "AZURE_DEVOPS_DEFAULT_PROJECT": "NflPlayoffPool"
      },
      "autoApprove": [
        "get_work_items",
        "create_work_item",
        "update_work_item",
        "get_builds",
        "queue_build",
        "get_releases",
        "create_release"
      ],
      "disabled": false
    }
  }
}
```

### Authentication Setup

#### Option A: Personal Access Token (Easiest)

1. **Create a PAT in Azure DevOps:**
   - Go to Azure DevOps → User Settings → Personal Access Tokens
   - Click "New Token"
   - Set scopes: Build (read & execute), Release (read, write & execute), Work Items (read & write)
   - Copy the token

2. **Update your configuration** with the PAT

#### Option B: Azure CLI Authentication

1. **Install Azure CLI** if not already installed
2. **Login:** `az login`
3. **Update MCP config:**
```json
"env": {
  "AZURE_DEVOPS_AUTH_METHOD": "azure-cli",
  "AZURE_DEVOPS_ORG_URL": "https://dev.azure.com/YourOrganization",
  "AZURE_DEVOPS_DEFAULT_PROJECT": "NflPlayoffPool"
}
```

## Option 2: Official Microsoft Azure DevOps MCP Server

### Installation with uvx

```bash
uvx @azure/mcp-server-azure-devops
```

### MCP Configuration

```json
{
  "mcpServers": {
    "azure-devops-official": {
      "command": "uvx",
      "args": ["@azure/mcp-server-azure-devops"],
      "env": {
        "AZURE_DEVOPS_ORG_URL": "https://dev.azure.com/YourOrganization",
        "AZURE_DEVOPS_PAT": "your-personal-access-token"
      },
      "autoApprove": ["*"],
      "disabled": false
    }
  }
}
```

## Available Tools

Once configured, you'll have access to these Azure DevOps tools:

### Work Items
- `get_work_items` - Get work items by query
- `create_work_item` - Create new work items
- `update_work_item` - Update existing work items
- `get_work_item_types` - Get available work item types

### Builds & Pipelines
- `get_builds` - Get build history
- `queue_build` - Trigger new builds
- `get_build_definition` - Get pipeline definitions
- `get_build_logs` - Get build logs

### Releases
- `get_releases` - Get release history
- `create_release` - Create new releases
- `get_release_definitions` - Get release pipeline definitions

### Repositories
- `get_repositories` - List repositories
- `get_commits` - Get commit history
- `create_pull_request` - Create pull requests
- `get_pull_requests` - Get pull request list

### Projects
- `get_projects` - List projects
- `get_project_properties` - Get project details

## Usage Examples

Once the MCP server is configured, you can use it in Kiro:

### Trigger a Build
```
"Queue a build for the main branch of NflPlayoffPool project"
```

### Check Build Status
```
"Get the latest build status for NflPlayoffPool"
```

### Create Work Items
```
"Create a bug work item for the login issue we discussed"
```

### Deploy to Environment
```
"Create a release to deploy build 123 to the test environment"
```

## Integration with Your VM Setup

With the Azure DevOps MCP server, you can:

1. **Automate VM Setup:** Trigger the setup pipeline for new VMs
2. **Monitor Deployments:** Check build and release status
3. **Create Work Items:** Automatically create tasks for deployment issues
4. **Update Work Items:** Mark deployment tasks as complete

### Example Workflow

```
1. "Create a work item to set up test environment VM"
2. "Queue the setup-vm pipeline with test environment parameters"
3. "Check if the VM setup build completed successfully"
4. "If successful, queue the deployment pipeline"
5. "Update the work item with deployment results"
```

## Troubleshooting

### Common Issues

**Authentication Failed:**
- Verify your PAT has the correct scopes
- Check that your organization URL is correct
- Ensure the PAT hasn't expired

**MCP Server Not Starting:**
- Check Node.js is installed and accessible
- Verify the path to the MCP server files
- Check the logs in Kiro's MCP panel

**Tools Not Available:**
- Restart Kiro after configuration changes
- Check the MCP server status in Kiro
- Verify autoApprove settings include the tools you need

### Getting Help

- Check the [Azure DevOps MCP Server repository](https://github.com/Tiberriver256/mcp-server-azure-devops) for issues
- Review Azure DevOps API documentation
- Check Kiro's MCP server logs for detailed error messages

This setup will give you powerful automation capabilities for your NFL Playoff Pool deployment pipeline!