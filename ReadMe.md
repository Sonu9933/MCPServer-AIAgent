# MCPServer + AIAgent

A **.NET 10** solution that demonstrates how to build an AI agent using the **Model Context Protocol (MCP)**. The solution contains two projects:

| Project | Role |
|---------|------|
| `MCPServer` | An MCP Server exposing document tools, resources, and prompts over stdio |
| `AIAgent` | An interactive console AI agent that connects to the MCP Server and an AI model |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An AI model provider account (see [Supported Providers](#supported-providers))
- *(Optional)* Node.js — for running the MCP Inspector

---

## Solution Structure

```
MCPServer+AIAgent.slnx
├── MCPServer/                               ← MCP Server project
│   ├── Program.cs                           ← Bootstraps the MCP server (stdio transport)
│   ├── InMemoryData/
│   │   └── DocsInMemory.cs                  ← In-memory document store (6 fake docs)
│   └── MCP/
│       ├── Tool/DocumentMCPTool.cs          ← ReadDoc + UpdateDoc tools
│       ├── Resource/DocumentMCPResource.cs  ← docs://documents URI resources
│       └── Prompt/DocumentMCPPrompt.cs      ← Prompt templates
└── AIAgent/                                 ← AI Agent project
    ├── Program.cs                           ← Host bootstrap
    ├── appsettings.json                     ← Provider configuration
    └── Agent/
        ├── AgentService.cs                  ← Chat loop + MCP client wiring
        └── ProviderFactory.cs               ← Builds the IChatClient from config
```

---

## Running the MCP Server (standalone)

The MCP Server communicates over **stdio** (JSON-RPC). Running it standalone will block — it needs an MCP client to talk to.

```powershell
cd MCPServer
dotnet run
```

### Testing with VS Code Copilot

1. Open the project in VS Code with the GitHub Copilot extension installed.
2. Click **Configure Tools...** in the Copilot panel → scroll to `stdio-coordinate-server` → select all tools → click **OK**.
3. Use prompts (prefix `/`) to call MCP Prompts, and the `+` icon in chat to attach MCP Resources.

### Testing with the MCP Inspector

```powershell
npx @modelcontextprotocol/inspector dotnet run --project MCPServer/MCPServer.csproj
```

---

## Running the AI Agent

### Step 1 — Configure your provider

Edit `AIAgent/appsettings.json` and set `Agent:Provider` to one of the supported values, then fill in the corresponding credentials section:

```json
{
  "Agent": {
    "Provider": "OpenAI"
  },
  "OpenAI": {
    "ApiKey": "sk-...",
    "Model": "gpt-4o"
  }
}
```

See [Supported Providers](#supported-providers) for all options.

### Step 2 — Run the agent

```powershell
cd AIAgent
dotnet run
```

The agent automatically launches the MCP Server as a child process, discovers its tools, and starts an interactive chat session.

### Example queries

```
You: List all available documents
You: Read the contents of report.pdf
You: Update plan.md with "Phase 1 complete, starting Phase 2"
You: exit
```

---

## Supported Providers

Switch providers by changing `Agent:Provider` in `appsettings.json` — **no code changes needed**.

### OpenAI *(default)*

```json
"Agent": { "Provider": "OpenAI" },
"OpenAI": {
  "ApiKey": "sk-...",
  "Model": "gpt-4o"
}
```

Supported models: `gpt-4o`, `gpt-4o-mini`, `gpt-4-turbo`, `gpt-3.5-turbo`

---

### Azure OpenAI

```json
"Agent": { "Provider": "AzureOpenAI" },
"AzureOpenAI": {
  "Endpoint": "https://<your-resource>.openai.azure.com/",
  "ApiKey": "<your-key>",
  "DeploymentName": "gpt-4o"
}
```

Same models as OpenAI, hosted in your Azure subscription.

---

### Ollama *(local, free, no API key)*

1. Install from [https://ollama.com](https://ollama.com)
2. Pull a model and start the server:
   ```powershell
   ollama pull llama3.2
   ollama serve
   ```
3. Configure:
   ```json
   "Agent": { "Provider": "Ollama" },
   "Ollama": {
     "Endpoint": "http://localhost:11434/v1",
     "Model": "llama3.2"
   }
   ```

Supported models: `llama3.2`, `mistral`, `phi4`, `qwen2.5`, `deepseek-r1`, and any model available via `ollama pull`.

---

### GitHub Models *(free tier)*

1. Create a GitHub Personal Access Token at [github.com/settings/tokens](https://github.com/settings/tokens)
2. Configure:
   ```json
   "Agent": { "Provider": "GitHub" },
   "GitHub": {
     "Token": "github_pat_...",
     "Model": "gpt-4o"
   }
   ```

Supported models: `gpt-4o`, `Phi-4`, `Meta-Llama-3.3-70B-Instruct`, and others at [github.com/marketplace/models](https://github.com/marketplace/models).

---

## Provider Comparison

| Provider      | Cost           | Data Privacy       | API Key Required |
|---------------|----------------|--------------------|------------------|
| OpenAI        | Pay-per-token  | Cloud (OpenAI)     | Yes              |
| Azure OpenAI  | Pay-per-token  | Your Azure tenant  | Yes              |
| Ollama        | Free           | Fully local        | No               |
| GitHub Models | Free tier      | Cloud (Microsoft)  | GitHub PAT       |

---

## MCP Primitives

| Primitive    | What it does                      | Example                              |
|--------------|-----------------------------------|--------------------------------------|
| **Tool**     | Function the AI can *call*        | `ReadDoc(docId)`, `UpdateDoc(...)`   |
| **Resource** | Data the AI can *read* by URI     | `docs://documents/{docId}`           |
| **Prompt**   | Reusable prompt templates         | `read_document_content_prompt`       |

---

## Dependencies

| Project   | Package                          | Version | Purpose                          |
|-----------|----------------------------------|---------|----------------------------------|
| MCPServer | `ModelContextProtocol`           | 1.2.0   | MCP server primitives            |
| MCPServer | `Microsoft.Extensions.Hosting`   | 10.0.5  | Host / DI                        |
| AIAgent   | `Microsoft.Extensions.AI`        | 9.5.0   | AI abstraction layer             |
| AIAgent   | `Microsoft.Extensions.AI.OpenAI` | 10.3.0  | OpenAI + Azure OpenAI integration|
| AIAgent   | `Azure.AI.OpenAI`                | 2.1.0   | Azure OpenAI client              |
| AIAgent   | `ModelContextProtocol`           | 1.2.0   | MCP client                       |
| AIAgent   | `Microsoft.Extensions.Hosting`   | 10.0.5  | Host / DI / Configuration        |