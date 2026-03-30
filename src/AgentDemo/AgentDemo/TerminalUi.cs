using Spectre.Console;
using System.Text;

namespace AgentDemo;

public sealed class TerminalUi
{
    public static TerminalUi? Current { get; private set; }

    public TerminalUi()
    {
        Current = this;
    }

    private static readonly BoxBorder DefaultBorder = BoxBorder.Rounded;
    private static readonly Style AccentStyle = new(Color.CornflowerBlue);

    public void ShowBanner()
    {
        var banner = new Panel(
            new Markup("[bold green]HelperBot[/]\n[grey]Type a question and press Enter (empty line to exit).[/]"))
        {
            Border = DefaultBorder,
            Header = new PanelHeader("[bold deepskyblue1]Chat Assistant[/]", Justify.Center),
            Padding = new Padding(1, 0, 1, 0)
        };

        banner.BorderStyle = AccentStyle;

        AnsiConsole.Write(banner);
        AnsiConsole.WriteLine();
    }

    public static Panel CreateAssistantPanel(string message, bool includeMarkup = false)
    {
        var safeMessage = includeMarkup? message : $"[white]{Markup.Escape(message)}[/]";

        var panel = new Panel(new Markup(safeMessage))
        {
            Header = new PanelHeader("[bold yellow]Assistant[/]", Justify.Left),
            Border = DefaultBorder,
            Padding = new Padding(1, 0, 1, 0)
        };

        panel.BorderStyle = new Style(Color.Yellow);
        return panel;
    }

    public string PromptQuestion()
    {
        AnsiConsole.MarkupLine("[bold cyan]You[/]");

        return AnsiConsole.Prompt(
            new TextPrompt<string>("[bold cyan]>[/] ")
                .AllowEmpty());
    }

    public void LogToolUse(string toolName, params (string Name, object? Value)[] inputs)
    {
        static string FormatValue(object? value)
        {
            return value is null
                ? "[grey](null)[/]"
                : $"[white]{Markup.Escape(value.ToString().Substring(0, Math.Min(value.ToString().Length, 20)))}[/]";
        }

        var details = inputs.Length == 0
            ? "[grey](no inputs)[/]"
            : string.Join("[orange1], [/]", inputs.Select(input => $"[bold]{Markup.Escape(input.Name)}[/]= {FormatValue(input.Value)}"));

        AnsiConsole.MarkupLine($"[orange1][bold]Tool[/] {Markup.Escape(toolName)} [grey]-[/] {details}[/]");
    }

    public async Task WriteAnswerAsync(IAsyncEnumerable<string> chunks)
    {
        AnsiConsole.WriteLine();
        
        var panel = CreateAssistantPanel(string.Empty);

        await AnsiConsole.Live(panel)
            .AutoClear(false)
            .StartAsync(async ctx =>
            {
                var loader = new Loader(ctx);
                var fullResponse = new StringBuilder();
                
                try
                {
                    loader.Start();
                    
                    await foreach (var chunk in chunks)
                    {
                        if (loader.IsRunning)
                        {
                            await loader.Stop();
                        }

                        fullResponse.Append(chunk);
                        ctx.UpdateTarget(CreateAssistantPanel(fullResponse.ToString()));
                    }
                }
                finally
                {
                    await loader.Stop();
                    ctx.UpdateTarget(CreateAssistantPanel(fullResponse.ToString()));
                }
            });

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
            Border = DefaultBorder,
            Padding = new Padding(1, 0, 1, 0)
        };

        panel.BorderStyle = new Style(Color.Green);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }
}