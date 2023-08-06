using RucSu;

namespace ArchiveLib;

public class ScheduleService
{
    private readonly Dictionary<DateTime, Day?> _days = new();

    private readonly HttpClient _httpClient;

    public ScheduleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Day?> GetDay(DateTime date, CancellationToken cancellationToken = default)
    {
        date = date.Date;

        if (_days.ContainsKey(date))
            return _days[date];

        List<Day>? days = await Parser.ScheduleAsync(
            _httpClient,
            date,
            File.ReadAllText("parameters.txt"),
            false,
            cancellationToken);

        if (days is null)
            return null;

        foreach (Day day in days)
            if (_days.ContainsKey(day.Date)) _days[day.Date] = day;
            else _days.Add(day.Date, day);

        if (!_days.ContainsKey(date)) _days.Add(date, null);

        return _days[date];
    }
}
