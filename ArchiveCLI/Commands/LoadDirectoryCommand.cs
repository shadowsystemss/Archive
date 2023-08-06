using ArchiveLib;
using CommandSystem;

namespace ArchiveCLI.Commands;

internal class LoadDirectoryCommand : ITextCommand, IMessageCommand
{
    private readonly Manager _manager;

    private string? DirectoryPath;

    private bool Recursive = true;
    private bool AutoAccept;
    private bool Logging = true;

    public LoadDirectoryCommand(Manager manager)
        => _manager = manager;

    public string Message => $"""
        Путь к директории: {DirectoryPath}.
        Выберите одну из опций:
        r - переключить рекурсию;
        a - переключить авто-соглашение с путём;
        l - переключить логгирование;
        s - запустить загрузку.
        """;

    public ICommand? Execute() => this;

    public ICommand? Execute(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        switch (text)
        {
            case "r":
                Recursive = !Recursive;
                return new MessageCommand(Recursive 
                    ? "Рекурсия включена."
                    : "Рекурсия отключена.",this);
            case "a":
                AutoAccept = !AutoAccept;
                return new MessageCommand(AutoAccept 
                    ? "Выбран режим авто."
                    : "Выбран режим ручной.", this);
            case "l":
                Logging = !Logging;
                return new MessageCommand(Logging
                    ? "Логи будут выводится."
                    : "Логгирование отключено.", this);
            case "s":
                {
                    if (!Directory.Exists(DirectoryPath))
                        return new MessageCommand("Директория не существует!");

                    LoadDirectory(DirectoryPath);
                    return new MessageCommand("Загрузка завершена.");
                }
        }
        return this;
    }

    public void LoadDirectory(string path)
    {
        if (Recursive)
            foreach (string directory in Directory.GetDirectories(path))
                LoadDirectory(directory);

        foreach (string file in Directory.GetFiles(path))
        {
            string destFilePath = _manager.GetPath(file).Result;
            string directory = Path.GetDirectoryName(destFilePath)
                   ?? throw new Exception("Folder not found in LoadDirectory()");

            // Если файл существует добавить, поменять имя.
            if (File.Exists(destFilePath))
            {
                int copyId = 0;
                string name = Path.GetFileNameWithoutExtension(destFilePath);
                string extension = Path.GetExtension(destFilePath);
                while (File.Exists(Path.Combine(directory, name + $" ({copyId})" + extension))) copyId++;
                destFilePath = Path.Combine(directory, name + $" ({copyId})" + extension);
            }

            if (!AutoAccept || Logging)
                Console.WriteLine(destFilePath);
            
            if (!AutoAccept)
            {
                Console.WriteLine("Согласиться с новым путём? (y/n)");
                if (Console.ReadLine() != "y")
                    continue;
            }
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.Move(file, destFilePath);
        }
    }

    public ICommand WithPath(string path)
    {
        DirectoryPath = path;
        return this;
    }
}
