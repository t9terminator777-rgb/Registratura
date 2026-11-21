List<Patient> queue = new();
int lastTicket = 0;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 — Встать в очередь | 2 — Вызвать следующего | 3 — Табло");
    Console.WriteLine("4 — Где я в очереди | 5 — Уйти из очереди | 6 — Без очереди | 0 — Выход");
    Console.Write("> ");

    switch (Console.ReadLine()?.Trim())
    {
        case null:
        case "0":
            return;
        case "1":
            AddPatient(null);
            break;
        case "2":
            if (queue.Count == 0)
            {
                Console.WriteLine("Очередь пуста.");
                break;
            }

            Patient next = queue[0];
            queue.RemoveAt(0);
            Console.WriteLine($"Талон {next.Ticket}, {next.Name}, пройдите в кабинет.");
            break;
        case "3":
            Console.WriteLine($"В очереди {queue.Count} чел.");
            foreach (Patient patient in queue)
            {
                string priority = patient.Priority is null ? "" : $" ({patient.Priority})";
                Console.WriteLine($"Талон {patient.Ticket}: {patient.Name}{priority}");
            }
            break;
        case "4":
            int position = FindTicket();
            if (position >= 0)
                Console.WriteLine($"Перед вами {position} чел.");
            break;
        case "5":
            int index = FindTicket();
            if (index >= 0)
            {
                queue.RemoveAt(index);
                Console.WriteLine("Талон удалён из очереди.");
            }
            break;
        case "6":
            Console.WriteLine("1 — Ветеран | 2 — Беременная | 3 — Родитель с ребёнком до года");
            Console.Write("Категория: ");
            string? category = Console.ReadLine()?.Trim() switch
            {
                "1" => "ветеран",
                "2" => "беременная",
                "3" => "родитель с ребёнком до года",
                _ => null
            };

            if (category is null)
                Console.WriteLine("Нет такой категории.");
            else
                AddPatient(category);
            break;
        default:
            Console.WriteLine("Нет такой команды.");
            break;
    }
}

void AddPatient(string? priority)
{
    Console.Write("Имя: ");
    string? name = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(name))
    {
        Console.WriteLine("Имя должно быть заполнено.");
        return;
    }

    Patient patient = new() { Ticket = ++lastTicket, Name = name, Priority = priority };
    int position = queue.Count;
    if (priority is null)
    {
        queue.Add(patient);
    }
    else
    {
        // Новый льготник встаёт перед первым обычным пациентом.
        int firstRegular = queue.FindIndex(p => p.Priority is null);
        if (firstRegular >= 0)
            position = firstRegular;

        queue.Insert(position, patient);
    }

    Console.WriteLine($"Ваш талон: {patient.Ticket}. Перед вами {position} чел.");
}

int FindTicket()
{
    Console.Write("Номер талона: ");
    if (!int.TryParse(Console.ReadLine(), out int ticket) || ticket <= 0)
    {
        Console.WriteLine("Введите положительный номер талона.");
        return -1;
    }

    int index = queue.FindIndex(p => p.Ticket == ticket);
    if (index == -1)
        Console.WriteLine("Талон не найден.");

    return index;
}
