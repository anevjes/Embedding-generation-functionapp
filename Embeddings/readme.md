# Function App Environment Vairables (Mandatory)
```
"FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
"AZURE_OPENAI_ENDPOINT": "https://<yourOpenAIEndpoint>.openai.azure.com/" OR "YOUR APIM ENDPOINT THAT EXPOSES OPENAI APIS",
"OPENAI_API_KEY":"< yourOpenAI API KEY>",
"EMBEDDING_MODEL_DEPLOYMENT_NAME": "text-embedding-ada-002",
"GENERATE_EMBEDDING_ENDPOINT":"http://<yourfuncitonapphost>/api/generate_embeddings"
```

# Input Shape\:

```json
{
    "values": [
      {
        "recordId": "a1",
        "data":
           {
             "contractText": 
                "This is a contract that was issues on November 3, 2023 and that involves... "
           }
      },
      {
        "recordId": "b5",
        "data":
           {
             "contractText": 
                "In the City of Seattle, WA on February 5, 2018 there was a decision made..."
           }
      },
      {
        "recordId": "c3",
        "data":
           {
             "contractText": null
           }
      }
    ]
}
```