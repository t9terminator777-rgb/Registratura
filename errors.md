# Ошибки из раздела 2

В опубликованной первой версии уже стояли проверки пустой очереди и `TryParse`. Для эксперимента ниже проверены небезопасные операции из стартового примера Notion. Номера строк для ввода талона относятся к учебному примеру под таблицей.

| Действие | Исключение | Сообщение .NET | Строка |
| --- | --- | --- | --- |
| Нажать 2 при пустой очереди | `ArgumentOutOfRangeException` | `Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')` | 26 в стартовом `Program.cs` из Notion |
| Нажать 4 и ввести `abc` | `FormatException` | `The input string 'abc' was not in a correct format.` | 2 в примере |
| Нажать 4 и ввести `999999999999999999999` | `OverflowException` | `Value was either too large or too small for an Int32.` | 2 в примере |

```csharp
string input = Console.ReadLine() ?? "";
int ticket = int.Parse(input);
```

В итоговой программе пустая очередь обрабатывается через `EmptyQueueException`, для номера применяется `TryParse`, а при отсутствии талона выводится сообщение из `TicketNotFoundException`. После каждого случая можно продолжать работу с очередью.
