namespace ConsoleTools.Reading
{
    public interface ICleanupComposer<TComposer>
    {
        CleanupConfiguration Cleanup { get; }
        TComposer WithCleanup(CleanupConfiguration cleanup);
    }
}
