using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;

namespace AIAgent.Agent;

public class AgentService
{
    private readonly IConfiguration _config;

    public AgentService(IConfiguration config)
    {
        _config = config;
    }

    public async Task RunAsync()
    {
        // 1. Build the AI chat client from the configured provider
        var provider = _config["Agent:Provider"] ?? "OpenAI";
        PrintColored($"Provider : {provider}", ConsoleColor.DarkGray);

        IChatClient chatClient = ProviderFactory
            .Create(_config)
            .AsBuilder()
            .UseFunctionInvocation()   // auto-calls MCP tools when the model requests them
            .Build();

        // 2. Locate and launch the MCP Server as a child process over stdio
        //   AppContext.BaseDirectory = AIAgent/bin/Debug/net10.0/
        //   4 levels up = solution root
        var mcpProjectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../MCPServer/MCPServer.csproj"));

        PrintColored("Starting MCP Server...", ConsoleColor.Cyan);

        await using var mcpClient = await McpClient.CreateAsync(
            new StdioClientTransport(new StdioClientTransportOptions
            {
                Command   = "dotnet",
                Arguments = ["run", "--project", mcpProjectPath, "--no-launch-profile"],
                Name      = "DocumentMCPServer"
            }));

        // 3. Discover tools exposed by the MCP Server
        var mcpTools = await mcpClient.ListToolsAsync();

        PrintColored(
            $"Connected! {mcpTools.Count} tool(s) available: " +
            string.Join(", ", mcpTools.Select(t => t.Name)),
            ConsoleColor.Green);

        // 6. Seed the conversation with a system prompt
        var history = new List<ChatMessage>
        {
            new(ChatRole.System,
                "You are a helpful document assistant. " +
                "Use the available tools to read and update documents when the user requests it. " +
                "Always confirm the outcome of any action you perform.")
        };

        Console.WriteLine();
        Console.WriteLine("AI Agent ready. Type your query, or type 'exit' to quit.");
        Console.WriteLine(new string('─', 60));

        // 7. Chat loop
        while (true)
        {
            PrintColored("\nYou: ", ConsoleColor.Yellow);
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            history.Add(new ChatMessage(ChatRole.User, input));

            // Call the model; UseFunctionInvocation handles any tool round-trips
            var response = await chatClient.GetResponseAsync(history, new ChatOptions
            {
                Tools = [.. mcpTools]
            });

            Console.WriteLine();
            PrintColored("Agent: ", ConsoleColor.Cyan);
            Console.WriteLine(response.Text);

            // Keep the assistant's reply in history for multi-turn context
            history.Add(new ChatMessage(ChatRole.Assistant, response.Text));
        }

        Console.WriteLine("\nGoodbye!");
    }

    private static void PrintColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
}
