namespace ConsoleTools.Validation
{
    public interface IValidatorComposer<T, out TComposer>
    {
        IValidator<T> Validator { get; }
        TComposer WithValidator(IValidator<T> validator);
    }
}
