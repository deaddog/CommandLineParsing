namespace CommandLineParsing.Input.Reading
{
    public class ReadlineCleanupConfiguration
    {
        public ReadlineCleanupConfiguration(ReadLineCleanup success, ReadLineCleanup cancel)
        {
            Success = success;
            Cancel = cancel;
        }

        public ReadLineCleanup Success { get; }
        public ReadLineCleanup Cancel { get; }
    }
}
