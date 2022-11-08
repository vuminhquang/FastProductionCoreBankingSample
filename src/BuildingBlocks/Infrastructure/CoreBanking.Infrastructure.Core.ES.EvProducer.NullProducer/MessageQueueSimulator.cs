using System.Diagnostics;
using EventSourcing.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiThreadsEngine;

namespace CoreBanking.Infrastructure.Core.ES.EvProducer.NullProducer;

public class MessageQueueSimulator : BackgroundService2
{
    private static MessageQueueSimulator? _instance;

    private static MessageQueueSimulator Instance
    {
        get
        {
            //Instance should not be null, as Background Service created at starting server
            Debug.Assert(_instance != null, nameof(_instance) + " != null");
            return _instance;
        }
        set => _instance ??= value; //only set once if _instance = null
    }
    
    public static class RegisterWrapper
    {
        public static void RegisterToRun(IIntegrationEvent job)
        {
            Console.WriteLine($"RegisterWrapper.RegisterToRun job {job}");
            Instance.RegisterToRun(job);
        }
    }

    private void RegisterToRun(IIntegrationEvent job)
    {
        _taskExecutor.AddJob(job);
    }
    
    private readonly MessageSender _taskExecutor;
    // private readonly IServiceScopeFactory _serviceScopeFactory;
    // private readonly IConfiguration _configuration;
    
    public MessageQueueSimulator(MessageSender taskExecutor)
    {
        Instance = this;

        // _serviceScopeFactory = serviceScopeFactory;
        _taskExecutor = taskExecutor;
        // _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingCts.Token.Register(() => _taskExecutor.Cancel());
        _taskExecutor.SetCancellationToken(stoppingToken);
    }
}