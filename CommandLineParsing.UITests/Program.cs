using NUnit.Framework;
using System;
using System.Collections.Generic;
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

            var results = new Dictionary<Type, bool>();

            foreground = Console.ForegroundColor;
            background = Console.BackgroundColor;

            foreach (var t in tests)
            {
                results.Add(t, ExecuteTest(t));
            }

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.CursorVisible = true;
            Console.Clear();

            var passedCount = results.Values.Count(x => x);
            var failedCount = results.Values.Count(x => !x);
            ColorConsole.WriteLine($"Tests results: [green:{passedCount} passed], [red:{failedCount} failed]");

            if (failedCount > 0)
            {
                ColorConsole.WriteLine("Failed tests:");
                foreach (var t in results.Where(x => !x.Value))
                {
                    var name = t.Key.GetCustomAttribute<TestNameAttribute>()?.Name ?? t.Key.Name;
                    ColorConsole.WriteLine("- " + name);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
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

        private static bool ExecuteTest(Type t)
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
                Console.Clear();
                ShowStateAndPressAnyKey("Test could not be loaded due to an exception: " + e.Message, ConsoleColor.Red);
                return false;
            }

            try
            {
                test.Execute();
            }
            catch (AssertionException e)
            {
                Console.Clear();
                ShowStateAndPressAnyKey("=================\n   Test failed   \n=================\n" + e.Message, ConsoleColor.Red);
                return false;
            }
            catch (Exception e)
            {
                Console.Clear();
                ShowStateAndPressAnyKey("Test execution failed due to an exception: " + e.Message, ConsoleColor.Red);
                return false;
            }

            return true;
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
