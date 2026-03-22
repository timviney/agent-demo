using AgentDemo;

var agent = await HelperBotAgent.CreateFromEnvironment();
var ui = new TerminalUi();

ui.ShowBanner();

while (true)
{
    var question = ui.PromptQuestion();

    await ui.WriteAnswerAsync(agent.RunStreamingAsync(question));
}
