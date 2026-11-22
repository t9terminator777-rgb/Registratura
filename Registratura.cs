sealed class Registratura
{
    private readonly List<Patient> _queue = [];
    private int _lastTicket;

    public IReadOnlyList<Patient> Queue => _queue;
    public Patient? Current { get; private set; }
    public Patient? Next => _queue.FirstOrDefault();

    public Patient Add(Citizen citizen, string phone, string? priority, bool hasOms, string card)
    {
        var patient = new Patient(++_lastTicket, citizen.Name, citizen.Pin, citizen.BirthDate,
            phone, priority, hasOms, card, DateTimeOffset.UtcNow);
        int position = priority is null ? -1 : _queue.FindIndex(item => item.Priority is null);
        _queue.Insert(position < 0 ? _queue.Count : position, patient);
        return patient;
    }

    public int FindTicket(int ticket)
    {
        int index = _queue.FindIndex(item => item.Ticket == ticket);
        return index >= 0 ? index : throw new TicketNotFoundException(ticket);
    }

    public void Leave(int ticket) => _queue.RemoveAt(FindTicket(ticket));

    public Patient CallNext()
    {
        if (Next is not { } patient)
            throw new EmptyQueueException();

        _queue.RemoveAt(0);
        return Current = patient;
    }

    public List<Patient> SendToPaidDesk()
    {
        var patients = _queue.FindAll(patient => !patient.HasOms);
        _queue.RemoveAll(patient => !patient.HasOms);
        return patients;
    }
}

sealed class TicketNotFoundException(int ticket) : Exception($"Талона {ticket} нет в очереди.");
sealed class EmptyQueueException() : Exception("Очередь пуста, вызывать некого.");
