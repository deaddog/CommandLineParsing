namespace ConsoleTools.Reading
{
    public static class CleanupComposerExtensions
    {
        public static TComposer WithCleanup<TComposer>(this ICleanupComposer<TComposer> composer, Cleanup cleanup)
        {
            return composer.WithCleanup(new CleanupConfiguration
            (
                success: cleanup,
                cancel: cleanup
            ));
        }
        public static TComposer WithCleanup<TComposer>(this ICleanupComposer<TComposer> composer, Cleanup success, Cleanup cancel)
        {
            return composer.WithCleanup(new CleanupConfiguration
            (
                success: success,
                cancel: cancel
            ));
        }
    }
}
