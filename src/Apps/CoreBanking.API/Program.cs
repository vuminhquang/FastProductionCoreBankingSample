using EngineFramework.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAllJsonFiles();

builder.Host.UseLamarAndPluginEngine((context, services) =>
{
    // Add services to the container.

    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.WriteTo.Console();
    // configuration.MinimumLevel.Debug();
    var logLevel = context.Configuration["Logging:LogLevel:Default"];
    Console.WriteLine($"Logging:LogLevel:Default {context.Configuration["Logging:LogLevel:Default"]}");
    if(string.IsNullOrEmpty(logLevel))
        switch (logLevel.ToLower())
        {
            case "debug":
                configuration.MinimumLevel.Debug();
                break;
        }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "false") //Not inside docker
{
    app.UseHttpsRedirection();
}
else if (app.Configuration["EnableNginx"] == "true")
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();