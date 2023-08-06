using RucSu;

namespace ArchiveLib;

public class Manager
{
    private readonly ScheduleService _scheduleService;

    public Manager(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public string Root { get; set; } = "C:\\Колледж";

    public async Task<string> GetPath(string path)
    {
        FileData data = new(path);

        Lesson? lesson = null;
        Day? day = null;
        if (data.FileNameDate.HasValue)
            day = await _scheduleService.GetDay(data.FileNameDate.Value);

        if (day != null
            && data.FileNameLessonName != null
            && data.FileNameLessonType != null)
        {
            lesson = day.Lessons?.Find(x =>
            x.Name == data.FileNameLessonName && GetLessonType(x) == data.FileNameLessonType);
            if (lesson != null)
                return GetPathByLesson(path, day.Date, lesson);
        }

        Day? day2 = await _scheduleService.GetDay(data.FileFirstDate);

        if (day2 != null
            && data.FileNameLessonName != null
            && data.FileNameLessonType != null)
        {
            lesson = day2.Lessons?.FirstOrDefault(x =>
            x.Name == data.FileNameLessonName && GetLessonType(x) == data.FileNameLessonType);
            if (lesson != null)
                return GetPathByLesson(path, day2.Date, lesson);
        }

        if (day != null && data.FileNameLessonName != null)
        {
            lesson = day.Lessons?.FirstOrDefault(x => x.Name == data.FileNameLessonName);
            if (lesson != null)
                return GetPathByLesson(path, day.Date, lesson);
        }

        int id = GetLessonId(data.FileFirstDate);
        lesson ??= day2?.Lessons?.Find(x => x.Id == id && x.Name == data.FileNameLessonName);

        if (day2 != null && data.FileNameLessonName != null)
        {
            lesson ??= day2.Lessons?.Find(x => x.Name == data.FileNameLessonName);
            if (lesson != null)
                return GetPathByLesson(path, day2.Date, lesson);
        }

        lesson ??= day2?.Lessons?.Find(x => x.Id == id);

        if (lesson != null && day2 != null)
            return GetPathByLesson(path, day2.Date, lesson);

        return Path.Combine(
                Root,
                "Не сортировано",
                GetFileTypeName(Path.GetExtension(path)),
                Path.GetFileName(path));
    }

    public string GetPathByLesson(string path, DateTime date, Lesson lesson)
        => Path.Combine(
            Root,
            lesson.Name,
            FirstCharUpper(GetLessonType(lesson)),
            date.Year.ToString(),
            date.ToString("MM (MMMM)"),
            date.ToString("dd"),
            GetFileTypeName(Path.GetExtension(path)),
            Path.GetFileName(path));

    public static int GetLessonId(DateTime date)
    {
        float time = date.Hour + date.Minute / 60;
        if (time < 8.5) return 0;
        if (time < 10.65) return 1;
        if (time < 12.5) return 2;
        if (time < 14.35) return 3;
        if (time < 16.25) return 4;
        if (time < 18.2) return 5;
        return 0;
    }

    public static string GetLessonType(Lesson lesson)
    {
        string type = lesson.Positions[0];
        return type.Remove(0, type.IndexOf(',') + 2);
    }

    public static string GetFileTypeName(string type) => type switch
    {
        ".mp3" => "Аудио",
        ".png" or ".jpg" or ".jpeg" => "Фото",
        ".mp4" => "Видео",
        ".txt" or ".docx" => "Документы",
        _ => "Прочее"
    };

    public static string FirstCharUpper(string value)
        => char.ToUpper(value[0]) + value[1..];
}
