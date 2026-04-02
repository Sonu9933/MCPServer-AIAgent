using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.MCP.Prompt
{
    [McpServerPromptType]
    public class DocumentMCPPrompt
    {
        [McpServerPrompt(Name = "read_document_content_prompt", Title = "Fetch document content Prompt")]
        [Description("A prompt to fetch document content.")]
        public async Task<string> GetDocumentContent(string docId)
        {
            return $"Content of the document {docId}:";
        }

        [McpServerPrompt(Name = "update_document_content_prompt", Title = "Update document content Prompt")]
        [Description("A prompt to update document content.")]
        public async Task<string> UpdateDocumentContent(string docId, string newDocData)
        {
            return $"Updated the document {docId} content with {newDocData}";
        }
    }
}
