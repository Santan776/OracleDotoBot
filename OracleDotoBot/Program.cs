﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OracleDotoBot.Abstractions;
using OracleDotoBot.Models;
using OracleDotoBot.RequestModels;
using OracleDotoBot.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Telegram.Bot;

public class Program
{
    private static async Task Main()
    {
        var builder = new ConfigurationBuilder();
        BuildConfig(builder);
        var config = builder
            .AddUserSecrets<Program>()
            .Build();

        var heroes = config.GetSection("AllHeroes:heroes");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var stratzApiLogger = new SerilogLoggerFactory(Log.Logger)
            .CreateLogger<IStratzApiService>();

        Log.Logger.Information("App starting");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ITelegramBotClient>(
                    x => new TelegramBotClient(config["Token"]));
                services.AddSingleton<IUserMatchesService, UserMatchesService>();
                services.AddSingleton<IStratzApiService>(new StratzApiService(config["StratzBaseUrl"], config["StratzToken"], stratzApiLogger));
                services.AddTransient<IResponseService, ResponseService>();
                services.AddHostedService<MessagesRecieverService>();
                services.Configure<List<Hero>>(heroes);
            })
            .UseSerilog()
            .Build();
        await host.RunAsync();
    }

    private static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddJsonFile("heroes.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}