using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenAI.Embeddings;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Arian.Sample.Output;
using Arian.Sample.Input;
using Azure.Core;
using System.Numerics;

namespace Arian.Sample;

public class EmbeddingsGenerator
{
    readonly ILogger<EmbeddingsGenerator> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private string _generateEmbeddingEndpoint = Environment.GetEnvironmentVariable("GENERATE_EMBEDDING_ENDPOINT");
    public EmbeddingsGenerator(ILogger<EmbeddingsGenerator> logger,IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
         _httpClientFactory = httpClientFactory;
    }

    internal class EmbeddingsRequest
    {
        [JsonPropertyName("RawText")]
        public string? RawText { get; set; }
    }


 [Function(nameof(GenerateEmbeddings_Start))]
    public async Task<HttpResponseData> GenerateEmbeddings_Start(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "embeddings")] HttpRequestData req,
        FunctionContext executionContext)
    {
        using StreamReader reader = new(req.Body);
        string request = await reader.ReadToEndAsync();
        OutputSkillSetData outputData = new OutputSkillSetData();
        outputData.Values = [];
        if (request != null)
        {
            //Serialize the input shape to match inputData object
            var skillsetInput = JsonSerializer.Deserialize<InputData>(request);
            HttpClient client = _httpClientFactory.CreateClient();

            
            foreach(var value in skillsetInput.values)
            { 
                //EmbeddingsRequest? embeddingsRequest = JsonSerializer.Deserialize<EmbeddingsRequest>(value.data.RawText);
                string rawTextJson = JsonSerializer.Serialize(value.data);
                var response = await client.PostAsync(_generateEmbeddingEndpoint, new StringContent(rawTextJson, System.Text.Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseContent);
                    List<float> vectorResult = new List<float>();
                    
                    if (embeddingResponse?.Data != null)
                    {
                        foreach (var data in embeddingResponse.Data)
                        {
                            if (data.Embedding != null)
                            {
                                vectorResult.AddRange(data.Embedding);
                            }
                        }
                    }


                    var responseValue = new Arian.Sample.Output.Value
                                        {
                                            RecordId = value.recordId,
                                            Data = new Arian.Sample.Output.Data
                                            {
                                                VectorData = vectorResult
                                            },
                                            Errors = new List<Error>(),
                                            Warnings = new List<Warning>()
                                        };
                    // outputData.Values = [responseValue];
                    outputData.Values.Add(responseValue);

                    _logger.LogInformation($"Called other function successfully: {responseContent}");
                }
                else
                {
                    // Handle error
                    _logger.LogError($"Error calling other function: {response.StatusCode}");
                }
            }
        }

        // Continue with your function's logic...
        var responseData = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await responseData.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(outputData)));
        await responseData.WriteStringAsync("Embedding generated successfully.");
        return responseData;
    }

    [Function(nameof(GenerateEmbeddings_Worker))]
    public async Task<HttpResponseData> GenerateEmbeddings_Worker(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "generate_embeddings")] HttpRequestData req,
        [EmbeddingsInput("{RawText}", InputType.RawText, Model = "%EMBEDDING_MODEL_DEPLOYMENT_NAME%")] EmbeddingsContext embeddings)
    {
        using StreamReader reader = new(req.Body);
        string request = await reader.ReadToEndAsync();

        EmbeddingsRequest? requestBody = JsonSerializer.Deserialize<EmbeddingsRequest>(request);

        // Check if requestBody is not null
        if (requestBody != null)
        {
            _logger.LogInformation(
                "Received {count} embedding(s) for input text containing {length} characters.",
                embeddings.Count,
                requestBody.RawText?.Length);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonSerializer.Serialize(embeddings.Response));

            return response;
        }
        else
        {
            // Handle the case where requestBody is null
            // For example, return a BadRequest response
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            string errorMessage = "Request body for emedding generation is missing or malformed.";
            response.WriteString(errorMessage);
            return response;
        }
    }
}