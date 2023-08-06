namespace ArchiveLib;

public class FileData
{
    public DateTime FileFirstDate { get; private set; }

    public string? FileNameLessonName { get; private set; }
    public string? FileNameLessonType { get; private set; }
    public DateTime? FileNameDate { get; private set; }

    public FileData(string path)
    {
        FileFirstDate = GetMinimalDate(path);

        LoadFileData(path);
    }

    private void LoadFileData(string path)
    {
        string name = Path.GetFileNameWithoutExtension(path);

        string[] data = name.Split(", ");

        if (data.Length == 0) return;
        FileNameLessonName = GetLessonFullName(data[0]);

        if (data.Length < 2) return;
        FileNameLessonType = data[1].Replace("практика", "практические занятия");

        if (data.Length > 2 && DateTime.TryParseExact(
            data[2],
            "dd.MM.yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out DateTime date))
            FileNameDate = date;
    }

    public static string GetLessonFullName(string name) => name.ToLower() switch
    {
        "обж" => "Основы безопасности жизнедеятельности",
        "вуп" => "Введение в управление проектами",
        _ => Manager.FirstCharUpper(name),
    };

    private static DateTime GetMinimalDate(string path)
    {
        DateTime creation = File.GetCreationTime(path);
        DateTime modify = File.GetLastWriteTime(path);
        return (modify < creation) ? modify : creation;
    }
}
