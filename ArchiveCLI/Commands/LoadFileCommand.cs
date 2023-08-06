using ArchiveLib;
using CommandSystem;

namespace ArchiveCLI.Commands;

internal class LoadFileCommand : ITextCommand, IMessageCommand
{
    private readonly Manager _manager;

    private string? FilePath;
    private string? SuggestedPath;

    public LoadFileCommand(Manager manager)
        => _manager = manager;

    public string Message
        => (FilePath is null || SuggestedPath is null)
        ? "Путь не указан."
        : $"""
        Предлагаемый путь: {SuggestedPath}
        Согласны ли с сохранить файл там? (y/n)
        """;

    public ICommand? Execute()
        => (FilePath is null || SuggestedPath is null) ? null : this;

    public ICommand? Execute(string? text)
    {
        if (FilePath is null || string.IsNullOrEmpty(text) || text == "n")
            return null;

        if (!File.Exists(FilePath))
            return new MessageCommand("Файл не найден.");

        if (text == "y")
        {
            if (SuggestedPath is null) return null;

            string directory = Path.GetDirectoryName(SuggestedPath)
                ?? throw new Exception("Folder not found in LoadFileCommand.Execute(text)");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            if (File.Exists(SuggestedPath))
            {
                int copyId = 0;
                string name = Path.GetFileNameWithoutExtension(SuggestedPath);
                string extension = Path.GetExtension(SuggestedPath);
                while (File.Exists(Path.Combine(directory, name + $" ({copyId})" + extension))) copyId++;
                SuggestedPath = Path.Combine(directory, name + $" ({copyId})" + extension);
            }
            File.Move(FilePath, SuggestedPath);
            return null;
        }
        return this;
    }

    public ICommand WithPath(string path)
    {
        FilePath = path;
        SuggestedPath = _manager.GetPath(FilePath).Result;
        return this;
    }
}
