using System.ClientModel;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using AgentDemo.Tools;
using Microsoft.Extensions.AI;

namespace AgentDemo;

public sealed class HelperBotAgent
{
    private AgentSession _session;
    private readonly AIAgent _agent;

    private HelperBotAgent(AIAgent agent, AgentSession session)
    {
        _agent = agent;
        _session = session;
    }

    public static async Task<HelperBotAgent> CreateFromEnvironment()
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
                instructions: "You are a helpful assistant that answers questions about the world. Use available tools when needed.",
                name: "HelperBot",
                tools: [
                    AIFunctionFactory.Create(Weather.GetWeather), 
                    AIFunctionFactory.Create(Weather.GetWeatherById), 
                    AIFunctionFactory.Create(UserDetails.GetUserDetails),
                    AIFunctionFactory.Create(GetContacts.GetContactsList),
                    AIFunctionFactory.Create(Places.SearchPlaces),
                    AIFunctionFactory.Create(Email.SendEmail),
                ]);

        var session = await agent.CreateSessionAsync();
        
        return new HelperBotAgent(agent, session);
    }

    public async IAsyncEnumerable<string> RunStreamingAsync(string question)
    {
        await foreach (var update in _agent.RunStreamingAsync(question, _session))
        {
            yield return update.ToString();
        }
    }
}
