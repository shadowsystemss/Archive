using CommandSystem;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiveCLI.Commands
{
    internal class PathInputCommand : ITextCommand, IMessageCommand
    {
        private readonly IServiceProvider _serviceProvider;

        public PathInputCommand(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public string Message => "Введите путь к файлу или директории.";

        public ICommand? Execute() => this;

        public ICommand? Execute(string? text)
        {
            if (File.Exists(text))
                return _serviceProvider.GetRequiredService<LoadFileCommand>().WithPath(text);
            else if (Directory.Exists(text))
                return _serviceProvider.GetRequiredService<LoadDirectoryCommand>().WithPath(text);
            else
                return new MessageCommand("Файл или директория не найден(а).");
        }
    }
}
