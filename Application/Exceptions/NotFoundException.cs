namespace Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Ресурс \"{name}\" с идентификатором ({key}) не найден в системе завода.") { }
}
