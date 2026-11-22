sealed record Patient(int Ticket, string Name, string Pin, DateOnly BirthDate, string Phone,
    string? Priority, bool HasOms, string Card, DateTimeOffset QueuedAt);
