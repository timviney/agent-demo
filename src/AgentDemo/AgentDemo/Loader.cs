using Spectre.Console;

namespace AgentDemo;

public class Loader
{
    private static readonly string[] Frames =
    {
        "|0     |",
        "|00    |",
        "|000   |",
        "| 000  |",
        "|  000 |",
        "|   000|",
        "|    00|",
        "|     0|",
    };

    private readonly object _gate = new();
    private CancellationTokenSource? _cts;
    private Task? _runTask;

    public bool IsRunning
    {
        get
        {
            lock (_gate)
            {
                return _runTask is { IsCompleted: false };
            }
        }
    }

    public void Start()
    {
        lock (_gate)
        {
            // Idempotent: ignore duplicate start while active.
            if (_runTask is { IsCompleted: false })
            {
                return;
            }

            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            _runTask = Task.Run(() => RunAsync(token), CancellationToken.None);
        }
    }

    public async Task Stop()
    {
        Task? runTask;
        CancellationTokenSource? cts;

        lock (_gate)
        {
            runTask = _runTask;
            cts = _cts;
            _runTask = null;
            _cts = null;
        }

        if (cts is null)
        {
            return;
        }

        try
        {
            await cts.CancelAsync().ConfigureAwait(false);
            if (runTask is not null)
            {
                await runTask.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during cancellation.
        }
        finally
        {
            cts.Dispose();
            AnsiConsole.Write("\r        \r");
        }
    }

    private static async Task RunAsync(CancellationToken token)
    {
        var i = 0;
        while (!token.IsCancellationRequested)
        {
            AnsiConsole.Markup($"[orange1]\r{Frames[i % Frames.Length]}[/]");
            i++;
            await Task.Delay(100, token).ConfigureAwait(false);
        }
    }
}