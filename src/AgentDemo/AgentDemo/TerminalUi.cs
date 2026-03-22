using Spectre.Console;

namespace AgentDemo;

public sealed class TerminalUi
{
    public static TerminalUi? Current { get; private set; }

    public TerminalUi()
    {
        Current = this;
    }

    public void ShowBanner()
    {
        AnsiConsole.MarkupLine("[bold green]HelperBot[/] [grey]ready.[/]");
        AnsiConsole.MarkupLine("[grey]Type a question and press Enter (empty line to exit).[/]");
    }

    public string PromptQuestion()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[bold cyan]>[/]")
                .AllowEmpty());
    }

    public async Task WriteAnswerAsync(IAsyncEnumerable<string> chunks)
    {
        AnsiConsole.WriteLine();

        var loader = new Loader();
        loader.Start();
        
        try
        {
            // Live stream pass
            await foreach (var chunk in chunks)
            {
                if (loader.IsRunning)
                {
                    await loader.Stop();
                }
                AnsiConsole.Write(chunk);
            }
        }
        finally
        {
            await loader.Stop();
        }

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    public void ShowEmailSent(string address, string subject, string body)
    {
        static string FormatValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "[grey](not provided)[/]"
                : $"[white]{Markup.Escape(value)}[/]";
        }

        var details = new Grid();
        details.AddColumn();
        details.AddColumn();
        details.AddRow("[bold deepskyblue1]To[/]", FormatValue(address));
        details.AddRow("[bold deepskyblue1]Subject[/]", FormatValue(subject));
        details.AddRow("[bold deepskyblue1]Body[/]", FormatValue(body));

        var panel = new Panel(details)
        {
            Header = new PanelHeader("[bold green]Email sent[/]", Justify.Center),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 0, 1, 0)
        };

        panel.BorderStyle = new Style(Color.Green);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }
}