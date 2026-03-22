using System.ClientModel;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;

namespace AgentDemo;

public sealed class HelperBotAgent
{
    private readonly AIAgent _agent;

    private HelperBotAgent(AIAgent agent)
    {
        _agent = agent;
    }

    public static HelperBotAgent CreateFromEnvironment()
    {
        var openRouterKey = Environment.GetEnvironmentVariable("OPEN_ROUTER_KEY")
            ?? throw new InvalidOperationException("OPEN_ROUTER_KEY environment variable is not set.");

        var openRouterClient = new OpenAIClient(
            new ApiKeyCredential(openRouterKey),
            new OpenAIClientOptions { Endpoint = new Uri("https://openrouter.ai/api/v1") }
        );

        const string model = "stepfun/step-3.5-flash:free";

        var agent = openRouterClient
            .GetChatClient(model)
            .AsAIAgent(
                instructions: "You are a helpful assistant that answers questions about the world. Remember to be a massive kiss ass",
                name: "HelperBot");

        return new HelperBotAgent(agent);
    }

    public async IAsyncEnumerable<string> RunStreamingAsync(string question)
    {
        await foreach (var update in _agent.RunStreamingAsync(question))
        {
            yield return update.ToString();
        }
    }
}

