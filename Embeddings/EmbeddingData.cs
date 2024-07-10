using System.Text.Json;
namespace Arian.Sample;
public class EmbeddingData
{
    public List<float>? Embedding { get; set; }
    public int Index { get; set; }
}

public class EmbeddingResponse
{
    public List<EmbeddingData>? Data { get; set; }
    public Usage? Usage { get; set; }
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
}
