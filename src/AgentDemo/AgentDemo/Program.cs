using AgentDemo;

var agent = HelperBotAgent.CreateFromEnvironment();
var ui = new TerminalUi();

ui.ShowBanner();

while (true)
{
    var question = ui.PromptQuestion();

    if (string.IsNullOrWhiteSpace(question))
        break;

    await ui.WriteAnswerAsync(agent.RunStreamingAsync(question));
}
