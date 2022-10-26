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
        // return;
        if(!Convert.ToBoolean(config["EnablePostgres"]) || !InDocker)
        {
            return;
        }
        
        await WaitForApplicationStarted();

        // await Cli.Wrap("bash")
        //     .WithArguments(args => args.Add("/usr/local/bin/docker-entrypoint.sh", true))
        //     .ExecuteAsync(stoppingToken);
        //
        // logger.LogInformation("/usr/local/bin/docker-entrypoint.sh");
        
        var task = StartPostgres(stoppingToken);

        // var publicUrl = await GetNgrokPublicUrl();
        // logger.LogInformation("Public ngrok URL: {NgrokPublicUrl}", publicUrl);
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

    private Task WaitForApplicationStarted()
    {
        var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        hostApplicationLifetime.ApplicationStarted.Register(() => completionSource.TrySetResult());
        return completionSource.Task;
    }

    private CommandTask<CommandResult> StartPostgres(CancellationToken stoppingToken)
    {
        var torTask = Cli.Wrap("postgres")
            // .WithArguments(args => args
            //     .Add("-g")
            //     .Add("daemon off;"))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(s => logger.LogDebug(s)))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(s => logger.LogError(s)))
            .ExecuteAsync(stoppingToken);
        return torTask;
    }
    
    private bool InDocker { get { return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";} }
}