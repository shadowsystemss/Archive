using ArchiveCLI.Commands;
using ArchiveCLI.Services;
using ArchiveLib;
using Microsoft.Extensions.DependencyInjection;

CommandsService commandsService = new ServiceCollection()
    // Services
    .AddSingleton<CommandsService>()
    .AddSingleton<Manager>()
    .AddSingleton<ScheduleService>()
    .AddSingleton<HttpClient>()
    // Commands
    .AddSingleton<PathInputCommand>()
    .AddSingleton<LoadFileCommand>()
    .AddTransient<LoadDirectoryCommand>()
    .AddSingleton<ClearCommand>()
    .AddSingleton<ExitCommand>()
    .BuildServiceProvider()
    .GetRequiredService<CommandsService>();

foreach (string arg in args)
    commandsService.Execute(arg);

while (true)
    commandsService.Execute(Console.ReadLine());