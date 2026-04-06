using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

namespace AIAgent.Agent;

public static class ProviderFactory
{
    /// <summary>
    /// Creates an <see cref="IChatClient"/> based on the <c>Agent:Provider</c>
    /// value in appsettings.json. Valid values: OpenAI | AzureOpenAI | Ollama | GitHub
    /// </summary>
    public static IChatClient Create(IConfiguration config)
    {
        var provider = (config["Agent:Provider"] ?? "OpenAI").ToUpperInvariant();

        return provider switch
        {
            "OPENAI"      => BuildOpenAI(config),
            "AZUREOPENAI" => BuildAzureOpenAI(config),
            "OLLAMA"      => BuildOllama(config),
            "GITHUB"      => BuildGitHub(config),
            _ => throw new InvalidOperationException(
                     $"Unknown provider '{config["Agent:Provider"]}'. " +
                     "Valid values: OpenAI, AzureOpenAI, Ollama, GitHub")
        };
    }

    // OpenAI
    private static IChatClient BuildOpenAI(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException(
                "OpenAI:ApiKey is not configured in appsettings.json.");

        var model = config["OpenAI:Model"] ?? "gpt-4o";

        return new OpenAIClient(apiKey)
            .GetChatClient(model)
            .AsIChatClient();
    }

    // Azure OpenAI
    private static IChatClient BuildAzureOpenAI(IConfiguration config)
    {
        var endpoint = config["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException(
                "AzureOpenAI:Endpoint is not configured in appsettings.json.");

        var apiKey = config["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException(
                "AzureOpenAI:ApiKey is not configured in appsettings.json.");

        var deployment = config["AzureOpenAI:DeploymentName"] ?? "gpt-4o";

        return new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deployment)
            .AsIChatClient();
    }

    // Ollama (local — OpenAI-compatible endpoint, no API key needed)
    private static IChatClient BuildOllama(IConfiguration config)
    {
        var endpoint = config["Ollama:Endpoint"] ?? "http://localhost:11434/v1";
        var model    = config["Ollama:Model"]    ?? "llama3.2";

        return new OpenAIClient(
                new ApiKeyCredential("ollama"),
                new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
            .GetChatClient(model)
            .AsIChatClient();
    }

    // GitHub Models
    private static IChatClient BuildGitHub(IConfiguration config)
    {
        var token = config["GitHub:Token"]
            ?? throw new InvalidOperationException(
                "GitHub:Token is not configured in appsettings.json.");

        var model = config["GitHub:Model"] ?? "gpt-4o";

        return new OpenAIClient(
                new ApiKeyCredential(token),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://models.inference.ai.azure.com")
                })
            .GetChatClient(model)
            .AsIChatClient();
    }
}
