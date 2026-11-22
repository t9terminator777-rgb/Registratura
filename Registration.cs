sealed class Registration(Registratura queue, GosReestr registry, Foms foms, Archive archive)
{
    public async Task<Patient> RegisterAsync(string pin, string phone, string? priority = null)
    {
        if (pin.Length != 14 || !pin.All(char.IsAsciiDigit))
            throw new ArgumentException("ПИН должен содержать 14 цифр.");

        Clock.Restart();
        var citizenTask = registry.FindAsync(pin);
        var policyTask = foms.HasPolicyAsync(pin);
        var cardTask = archive.FindCardAsync(pin);

        await Task.WhenAll(citizenTask, policyTask, cardTask);
        var citizen = await citizenTask
            ?? throw new ArgumentException("ПИН не найден в госреестре. Проверьте номер.");
        var patient = queue.Add(citizen, phone, priority, await policyTask, await cardTask);
        Clock.Log($"Готово: {patient.Name}, полис: {patient.HasOms}, карта: {patient.Card}");
        return patient;
    }

    public async Task CompareAsync(string pin)
    {
        Clock.Restart();
        await registry.FindAsync(pin);
        await foms.HasPolicyAsync(pin);
        await archive.FindCardAsync(pin);
        Clock.Log("Последовательные запросы завершены.");

        Clock.Restart();
        await Task.WhenAll(registry.FindAsync(pin), foms.HasPolicyAsync(pin), archive.FindCardAsync(pin));
        Clock.Log("Параллельные запросы завершены.");
    }
}
