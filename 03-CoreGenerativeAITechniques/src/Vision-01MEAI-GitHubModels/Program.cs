using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()  // 'Program' can be any class in your assembly
    .Build();

var deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT"] ??
    Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");

var endpoint = new Uri((configuration["AZURE_OPENAI_ENDPOINT"] ??
    Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"))!); // e.g. "https://< your hub name >.openai.azure.com/"

var apiKey = new ApiKeyCredential(
    (configuration["AZURE_AI_SECRET"] ?? Environment.GetEnvironmentVariable("AZURE_AI_SECRET"))!
    );

IChatClient chatClient = new AzureOpenAIClient(
    endpoint,
    apiKey)
.AsChatClient(deploymentName!);

// images
string imgRunningShoes = "running-shoes.jpg";
string imgCarLicense = "license.jpg";
string imgReceipt = "german-receipt.jpg";

// prompts
var promptDescribe = "Describe the image";
var promptAnalyze = "How many red shoes are in the picture? and what other shoes colors are there?";
var promptOcr = "What is the text in this picture? Is there a theme for this?";
var promptReceipt = "I bought the coffee and the sausage. How much do I owe? Add a 18% tip.";

// prompts
string systemPrompt = @"You are a useful assistant that describes images using a direct style.";
var prompt = promptDescribe;
string imageFileName = imgRunningShoes;
string image = Path.Combine(Directory.GetCurrentDirectory(), "images", imageFileName);

List<ChatMessage> messages =
[
    new ChatMessage(Microsoft.Extensions.AI.ChatRole.System, systemPrompt),
    new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, prompt),
];

// read the image bytes, create a new image content part and add it to the messages
AIContent aic = new DataContent(File.ReadAllBytes(image), "image/jpeg");
var message = new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [aic]);
messages.Add(message);

// send the messages to the assistant
var response = await chatClient.GetResponseAsync(messages);
Console.WriteLine($"Prompt: {prompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {response.Message}");
