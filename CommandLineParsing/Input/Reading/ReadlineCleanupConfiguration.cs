namespace CommandLineParsing.Input.Reading
{
    public class ReadlineCleanupConfiguration
    {
        public ReadlineCleanupConfiguration(ReadlineCleanup success, ReadlineCleanup cancel)
        {
            Success = success;
            Cancel = cancel;
        }

        public ReadlineCleanup Success { get; }
        public ReadlineCleanup Cancel { get; }
    }
}
