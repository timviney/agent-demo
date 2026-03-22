using System.Text;
using Spectre.Console;

namespace AgentDemo;

public sealed class TerminalUi
{
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
}