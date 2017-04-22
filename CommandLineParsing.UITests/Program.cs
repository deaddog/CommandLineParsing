using System;
using System.Linq;
using System.Reflection;

namespace CommandLineParsing.UITests
{
    class Program
    {
        static ConsoleColor foreground;
        static ConsoleColor background;

        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var tests = assembly.GetTypes().Where(IsTest).ToList();

            foreground = Console.ForegroundColor;
            background = Console.BackgroundColor;

            foreach (var t in tests)
            {
                var name = t.GetCustomAttribute<TestNameAttribute>()?.Name ?? t.Name;

                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
                Console.CursorVisible = true;
                Console.Clear();

                Console.Write("Press any key to start executing test ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(name);
                Console.ReadKey(true);

                Console.ForegroundColor = foreground;
                Console.Clear();

                UITest test = null;
                try
                {
                    test = (UITest)Activator.CreateInstance(t);
                }
                catch (Exception e)
                {
                    ShowStateAndPressAnyKey("Test could not be loaded due to an exception: " + e.Message, ConsoleColor.Red);
                    continue;
                }

                try
                {
                    test.Execute();
                }
                catch (Exception e)
                {
                    ShowStateAndPressAnyKey("Test execution failed due to an exception: " + e.Message, ConsoleColor.Red);
                    continue;
                }

                ShowStateAndPressAnyKey("Test executed successfully!", ConsoleColor.Green);
            }
        }

        private static bool IsTest(Type type)
        {
            return type != null && !type.IsInterface && !type.IsAbstract && IsTestBase(type);
        }
        private static bool IsTestBase(Type type)
        {
            if (type == null)
                return false;
            else if (type == typeof(UITest))
                return true;
            else
                return IsTestBase(type.BaseType);
        }

        private static void ShowStateAndPressAnyKey(string message, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        {
            Console.ForegroundColor = foregroundColor ?? foreground;
            Console.BackgroundColor = backgroundColor ?? background;
            Console.CursorVisible = true;

            Console.WriteLine();
            Console.WriteLine(message);

            Console.ForegroundColor = foreground;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
