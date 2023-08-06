using ArchiveCLI.Commands;
using CommandSystem;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiveCLI.Services
{
    internal class CommandsService : ITextCommand
    {
        private ICommand? _step;
        private readonly IServiceProvider _serviceProvider;

        public CommandsService(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        protected ICommand? StartCommand(string? text)
        => text switch
        {
            "load" => _serviceProvider.GetRequiredService<PathInputCommand>(),
            "clear" => _serviceProvider.GetRequiredService<ClearCommand>(),
            "exit" => _serviceProvider.GetRequiredService<ExitCommand>(),
            _ => null,
        };

        public ICommand? Execute(string? text)
        {
            if (_step is null)
                _step = StartCommand(text);
            else if (text == "exit")
                _step = null;
            else if (_step is ITextCommand textCommand)
                _step = textCommand.Execute(text);

            if (_step is IMessageCommand messageCommand)
                Console.WriteLine(messageCommand.Message);

            _step = _step?.Execute();

            return _step;
        }

        public ICommand? Execute() => _step;
    }
}
