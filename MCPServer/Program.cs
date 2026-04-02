using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MCPServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Logging.AddConsole(consoleLogOptions =>
            {
                // Configure all logs to go to stderr
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                // Scans for MCP Tools
                .WithToolsFromAssembly()
                // Scans for MCP Resources
                .WithResourcesFromAssembly()
                // Scans for MCP Prompts
                .WithPromptsFromAssembly();

            builder
                .Build()
                .Run();
        }
    }
}
