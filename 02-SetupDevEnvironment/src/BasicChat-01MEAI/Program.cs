using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddUserSecrets<Program>()  // 'Program' can be any class in your assembly
    .Build();

var deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT"] ??
    Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");

var endpoint = new Uri((configuration["AZURE_OPENAI_ENDPOINT"] ?? 
    Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"))!); // e.g. "https://< your hub name >.openai.azure.com/"

var apiKey = new ApiKeyCredential(
    (configuration["AZURE_AI_SECRET"] ?? Environment.GetEnvironmentVariable("AZURE_AI_SECRET"))!
    );

IChatClient client = new AzureOpenAIClient(
    endpoint,
    apiKey)
.AsChatClient(deploymentName!);

var response = await client.GetResponseAsync("What is AI?");

Console.WriteLine(response.Message);