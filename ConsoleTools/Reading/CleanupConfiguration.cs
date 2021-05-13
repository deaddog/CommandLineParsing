namespace ConsoleTools.Reading
{
    public class CleanupConfiguration
    {
        public CleanupConfiguration(Cleanup success, Cleanup cancel)
        {
            Success = success;
            Cancel = cancel;
        }

        public Cleanup Success { get; }
        public Cleanup Cancel { get; }
    }
}
