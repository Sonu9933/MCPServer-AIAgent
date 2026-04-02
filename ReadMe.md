# MCPServer

## Description
MCPServer is a .NET 10 application designed to provide a server implementation using the Model Context Protocol. It leverages the `Microsoft.Extensions.Hosting` library for hosting and includes tools and resources for managing documents in memory.

## Prerequisites
- .NET 10 SDK installed on your system.
- Node.js and npm installed for running the Model Context Protocol Inspector.

## How to Run
1. Run mcpServer project using visual studio or using the command line with `dotnet run` in the project directory.

2. Navigate to the directory where the `.csproj` file exists:

3. Run the MCP Inspector using the following command:
 
## Project Structure
- **MCPServer.csproj**: Project file defining dependencies and target framework.
- **Program.cs**: Entry point of the application. Configures the host and services.
- **MCP\Resource\DocumentMCPPrompt.cs**: Contains prompt related for document handling.
- **MCP\Resource\DocumentMCPResource.cs**: Contains resource-related logic for document handling.
- **MCP\Tool\DocumentMCPTool.cs**: Implements tools for document processing.
- **InMemoryData\DocsInMemory.cs**: Manages in-memory data storage for documents.

## Dependencies
- `Microsoft.Extensions.Hosting` (v10.0.5): Provides hosting capabilities.
- `ModelContextProtocol` (v1.2.0): Implements the Model Context Protocol.

## Entry Point
The application starts in `Program.cs` with the `Main` method:

## Notes
- Ensure all dependencies are restored before running the application.
- The `npx` command requires an active internet connection to fetch the `@modelcontextprotocol/inspector` package.
