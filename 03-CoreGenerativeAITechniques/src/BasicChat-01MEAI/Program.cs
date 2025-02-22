using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.Text;

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

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Message);
