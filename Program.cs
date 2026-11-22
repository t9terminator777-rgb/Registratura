Console.OutputEncoding = System.Text.Encoding.UTF8;
var queue = new Registratura();
var registration = new Registration(queue, new GosReestr(), new Foms(), new Archive());
var sms = new SmsSender();

if (args.Contains("--compare"))
{
    await registration.CompareAsync("21203199850123");
    return;
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 — Встать в очередь | 2 — Вызвать следующего | 3 — Табло");
    Console.WriteLine("4 — Где я в очереди | 5 — Уйти из очереди | 6 — Без очереди");
    Console.WriteLine("7 — Направить пациентов без ОМС в платную кассу | 0 — Выход");
    Console.Write("> ");
    string? command = Console.ReadLine()?.Trim();
    if (command is null or "0")
        return;

    try
    {
        switch (command)
        {
            case "1":
                await AddPatientAsync(null);
                break;
            case "2":
                var patient = queue.CallNext();
                Console.WriteLine($"Талон {patient.Ticket}, {patient.Name}, пройдите в кабинет.");
                if (queue.Next is { } next)
                    await sms.SendAsync(next.Phone, "Вы следующий, подойдите к кабинету");
                break;
            case "3":
                Console.WriteLine($"В кабинете: {queue.Current?.Name ?? "никого"}. Следующий: {queue.Next?.Name ?? "никого"}.");
                foreach (var entry in queue.Queue)
                {
                    var waiting = DateTimeOffset.UtcNow - entry.QueuedAt;
                    Console.WriteLine($"Талон {entry.Ticket}: {entry.Name} | {entry.Priority ?? "обычная очередь"} | ждёт {(int)waiting.TotalMinutes}:{waiting.Seconds:D2}");
                }
                break;
            case "4":
                if (ReadTicket() is int positionTicket)
                    Console.WriteLine($"Перед вами {queue.FindTicket(positionTicket)} чел.");
                break;
            case "5":
                if (ReadTicket() is int leaveTicket)
                {
                    queue.Leave(leaveTicket);
                    Console.WriteLine("Талон удалён из очереди.");
                }
                break;
            case "6":
                Console.WriteLine("1 — Ветеран | 2 — Беременная | 3 — Родитель с ребёнком до года");
                string? category = Console.ReadLine()?.Trim() switch
                {
                    "1" => "Ветеран",
                    "2" => "Беременная",
                    "3" => "Родитель с ребёнком до года",
                    _ => null
                };
                if (category is null)
                    Console.WriteLine("Выберите категорию от 1 до 3.");
                else
                    await AddPatientAsync(category);
                break;
            case "7":
                var sent = queue.SendToPaidDesk();
                foreach (var entry in sent)
                    Console.WriteLine($"Талон {entry.Ticket}, {entry.Name}: полиса ОМС нет, пройдите в платную кассу.");
                Console.WriteLine($"Направлено в платную кассу: {sent.Count}.");
                break;
            default:
                Console.WriteLine("Такой команды нет. Выберите номер из меню.");
                break;
        }
    }
    catch (TicketNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (EmptyQueueException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (GosReestrUnavailableException ex)
    {
        Clock.Log($"{ex.Message} Пациент не добавлен. Повторите попытку позже.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine(command == "2" ? $"SMS не отправлено: {ex.Message}" : ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Не удалось выполнить команду. Можно выбрать другую или повторить.");
        Console.Error.WriteLine(ex);
    }
    finally
    {
        Console.WriteLine($"[табло] В очереди {queue.Queue.Count} чел.");
    }
}

async Task AddPatientAsync(string? priority)
{
    Console.Write("ПИН: ");
    string pin = Console.ReadLine()?.Trim() ?? "";
    Console.Write("Телефон (можно пропустить): ");
    string phone = Console.ReadLine()?.Trim() ?? "";
    var patient = await registration.RegisterAsync(pin, phone, priority);
    Console.WriteLine($"Ваш талон: {patient.Ticket}. Перед вами {queue.FindTicket(patient.Ticket)} чел.");
}

static int? ReadTicket()
{
    Console.Write("Номер талона: ");
    if (int.TryParse(Console.ReadLine(), out int ticket) && ticket > 0)
        return ticket;

    Console.WriteLine("Введите положительный номер талона цифрами, например 15.");
    return null;
}
