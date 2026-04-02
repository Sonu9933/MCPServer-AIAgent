namespace MCPServer.InMemoryData
{
    public class DocsInMemory
    {

        public readonly IDictionary<string, string> docs;

        public DocsInMemory()
        {
            docs = new Dictionary<string, string>()
            {
                ["deposition.md"] = "This deposition covers the testimony of Angela Smith, P.E.",
                ["report.pdf"] = "The report details the state of a 20m condenser tower.",
                ["financials.docx"] = "These financials outline the project's budget and expenditures",
                ["outlook.pdf"] = "This document presents the projected future performance of the system",
                ["plan.md"] = "The plan outlines the steps for the project's implementation.",
                ["spec.txt"] = "These specifications define the technical requirements for the equipment"
            };
        }

        public async Task<string> ReadDoc(string docId)
        {
            if (!docs.TryGetValue(docId, out var doc))
            {
                return string.Empty;
            }

            return doc;
        }

        public async Task<string> UpdateDoc(string docId, string newDocData)
        {
            if (!docs.TryGetValue(docId, out _))
            {
                return "Not(Failure)";
            }

            docs[docId] = newDocData;

            return "Successfully";
        }

        public async Task<List<string>> FetchDocs()
        {
            return [.. docs.Keys];
        }

        public async Task<string> FetchDoc(string docId)
        {
            if (!docs.TryGetValue(docId, out _))
            {
                return string.Empty;
            }

            return docId;
        }
    }
}
