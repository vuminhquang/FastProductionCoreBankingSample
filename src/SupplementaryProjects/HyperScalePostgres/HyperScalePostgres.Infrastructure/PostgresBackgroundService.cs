using CliWrap;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HyperScalePostgres.Infrastructure;

public class PostgresBackgroundService : BackgroundService
{
    private readonly IServer server;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IConfiguration config;
    private readonly ILogger<PostgresBackgroundService> logger;
    private CancellationTokenSource? _cancellationTokenSource;
    
    public PostgresBackgroundService(
        IServer server,
        IHostApplicationLifetime hostApplicationLifetime,
        IConfiguration config,
        ILogger<PostgresBackgroundService> logger
    )
    {
        this.server = server;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.config = config;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(WhenStopByCancellation);
        _cancellationTokenSource = new CancellationTokenSource();
        
        if(!Convert.ToBoolean(config["EnablePostgres"]) || !InDocker)
        {
            return;
        }
        
        await WaitForApplicationStarted();

        var task = StartPostgres(_cancellationTokenSource.Token);
        
        logger.LogInformation("Starting Postgres");

        try
        {
            await task;
        }
        finally
        {
            logger.LogInformation("Postgres stopped");
        }
    }

    private void WhenStopByCancellation()
    {
        // var stopPostgresCommand = "kill -INT `head -1 /usr/local/pgsql/data/postmaster.pid`";
        if (_cancellationTokenSource is null) return;
        _cancellationTokenSource.CancelAfter(1000*60);//Will force after 1 min if postgres did not canceled
        Cli.Wrap("kill")
            .WithArguments(args => args
                .Add("-INT")
                .Add("`head -1 /usr/local/pgsql/data/postmaster.pid`", false)
            )
            .ExecuteAsync(_cancellationTokenSource.Token).GetAwaiter();
        logger.LogInformation("Shutdown Postgres Command Sent");
    }

    private Task WaitForApplicationStarted()
    {
        var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        hostApplicationLifetime.ApplicationStarted.Register(() => completionSource.TrySetResult());
        return completionSource.Task;
    }

    private CommandTask<CommandResult> StartPostgres(CancellationToken stoppingToken)
    {
        var torTask = Cli.Wrap("/usr/local/bin/docker-entrypoint.sh")
            .WithArguments(args => args
                .Add("postgres")
                /*.Add("daemon off;")*/)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(s => logger.LogDebug(s)))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(s => logger.LogError(s)))
            .ExecuteAsync(stoppingToken);
        return torTask;
    }
    
    private bool InDocker { get { return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";} }
}