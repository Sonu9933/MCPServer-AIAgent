using MCPServer.InMemoryData;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace MCPServer.MCP.Resource
{
    [McpServerResourceType]
    public class DocumentMCPResource
    {
        private readonly DocsInMemory inMemoryDocs;
        public DocumentMCPResource()
        {
            inMemoryDocs = new DocsInMemory();
        }

        [McpServerResource(UriTemplate = "docs://documents/{docId}", MimeType = "application/json")]
        [Description("A Resource to fetch doc based on doc id.")]
        public async Task<string> FetchDoc(string docId)
        {
            return JsonSerializer.Serialize(await inMemoryDocs.FetchDoc(docId));
        }

        [McpServerResource(UriTemplate = "docs://documents", MimeType = "application/json")]
        [Description("A Resource to fetch all the docs.")]
        public async Task<string> FetchDocs()
        {
            var docs = await inMemoryDocs.FetchDocs();
            return JsonSerializer.Serialize(docs);
        }
    }
}
