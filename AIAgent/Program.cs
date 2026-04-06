using AIAgent.Agent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Suppress framework noise — only show warnings and above
builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.Services.AddSingleton<AgentService>();

var host = builder.Build();

var agent = host.Services.GetRequiredService<AgentService>();
await agent.RunAsync();
