namespace ConsoleTools.Validation
{
    public interface IValidator<T>
    {
        Message Validate(T item);
    }
}
