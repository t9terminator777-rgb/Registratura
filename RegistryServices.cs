using System.Diagnostics;

sealed record Citizen(string Pin, string Name, DateOnly BirthDate);
sealed class GosReestrUnavailableException() : Exception("Госреестр не отвечает.");

static class Clock
{
    private static readonly Stopwatch Timer = Stopwatch.StartNew();

    public static void Restart() => Timer.Restart();
    public static void Log(string text) => Console.WriteLine($"[{Timer.Elapsed.TotalSeconds:F2} с] {text}");
}

sealed class GosReestr
{
    private readonly Citizen[] _citizens =
    [
        new("21203199850123", "Айбек Асанов", new(1998, 3, 12)),
        new("12507200350456", "Нургуль Токтосунова", new(2003, 7, 25)),
        new("20211198500789", "Бакыт Жумабаев", new(1985, 11, 2)),
        new("13001200150321", "Айпери Садыкова", new(2001, 1, 30)),
        new("21409200650654", "Тимур Ким", new(2006, 9, 14))
    ];

    public async Task<Citizen?> FindAsync(string pin)
    {
        Clock.Log("Госреестр: запрос...");
        await Task.Delay(1500);
        if (pin == "20101200000000")
            throw new GosReestrUnavailableException();

        Clock.Log("Госреестр: ответ получен");
        return Array.Find(_citizens, citizen => citizen.Pin == pin);
    }
}

sealed class Foms
{
    public async Task<bool> HasPolicyAsync(string pin)
    {
        Clock.Log("ФОМС: проверяю полис...");
        await Task.Delay(1000);
        Clock.Log("ФОМС: ответ получен");
        return pin is not ("12507200350456" or "20211198500789");
    }
}

sealed class Archive
{
    public async Task<string> FindCardAsync(string pin)
    {
        Clock.Log("Архив: ищу медкарту...");
        await Task.Delay(2000);
        Clock.Log("Архив: медкарта найдена");
        return $"МК-{pin[^5..]}";
    }
}

sealed class SmsSender
{
    public async Task SendAsync(string phone, string text)
    {
        await Task.Delay(1000);
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Не указан номер телефона");

        Console.WriteLine($"SMS на {phone}: {text}");
    }
}
