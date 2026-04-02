using MCPServer.InMemoryData;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace MCPServer.MCP.Tool
{
    [McpServerToolType]
    public class DocumentMCPTool
    {
        private readonly DocsInMemory inMemoryDocs;
        public DocumentMCPTool() 
        {
            inMemoryDocs = new DocsInMemory();
        }

        [McpServerTool, Description("Read the contents of a document and return it as a string.")]
        public async Task<string> ReadDoc(string docId)
        {
            var result = await inMemoryDocs.ReadDoc(docId);
            return JsonSerializer.Serialize(result); 
        }

        [McpServerTool, Description("Edit a document by replacing a string in the documents content with a new string.")]
        public async Task<string> UpdateDoc(string docId, string newDocData)
        {
            var result = await inMemoryDocs.UpdateDoc(docId, newDocData);
            return JsonSerializer.Serialize(result);
        }
    }
}
