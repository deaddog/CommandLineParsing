# CommandLineParsing
A .NET framework that provides a range of functionalities for creating command line applications using the .NET framework.
It's primary features are:

### Colored output
Instead of using the `System.Console` to produce output, the framework provides a `ColorConsole` class that provides a simple format to generate color-coded output.

    ColorConsole.WriteLine($"Something is [blue:colored] differently.");

This simplifies the process of dynamically changing color in output.
There is also support for defining custom coloring names, that then translate into one of the existing console colors.
This allows for a application-wide *example* color that can easily be redefined.

When dynamically producing output, the framework provides a `Formatter` class that supports variables, functions and conditional output.
This enables an end-user to specify a custom output format for some collection of data.

### Parsing
Any type that provides a `TryParse` method can be read directly from the console using a single command.
The function will not return until a value has been parsed from input and that value passes the (optional) validation.

    int age = ColorConsole.ReadLine<int>("Please enter your age: ");

The above prompts the user and awaits a number as input.
It can be extended to check that the input value is positive or in a certain range.
Because this system is based on the existance of `TryParse` methods custom types can be parsed as well if they declare the proper method.
For a class `MyClass` the method can be one of the following:

- `public static bool TryParse(string, out MyClass)`, which returns a generic message in case parsing fails.
- `public static Message MessageTryParse(string, out MyClass)`, which can return a custom message in case parsing fails.

The methods are loaded using reflection, so you **must be exact** when naming them in classes.

### Menus
When the user is to select an item from a predefined collection, a `Menu` (or `SelectionMenu` when selecting multiple items) can be used to display a menu to the user with the available options.
The displayed menu displays a *cursor* that can move up and down when selecting. Additionally items can be listed with shortcut keys a-z or 1-9.

Menus can be constructed manually (setting up each option and displaying the menu) or using a set of extension methods on `IEnumerable`. The latter allows for a one-liner when the user should select an element:

    string[] names = { "Abraham", "Benny", "Carl", "Dennis" };

    string selectedName = names.MenuSelect(null);
    string[] selectedNames = names.MenuSelectMultiple(null);

The `null` argument can be replaced by an object that describes settings for how the menu should be displayed.
Overall the idea is to encapsulate displaying a menu in as short a line as possible.

### Commands and parameters
Using types defined by the framework, commands and their parameters can be set up and then executed using the supplied command line arguments.
A command is defined by inheriting the `Command` class and declaring its parameters therein.
Parameters are initialized using reflection before executing the commands constructor and must, because of this, be declared `readonly`.

    public class MyCommand : Command
    {
      private readonly Parameter<int> magic;

      public override void Execute()
      {
        ColorConsole.WriteLine($"The magic number is {magic.Value}."};
      }
    }

Parameters supports parsing using the same system as `ReadLine` described above.
Additionally a parsing method can be specified for any individual parameter allowing for further customization.
Parameters also supports validation of the entered values, which can be specified as:

    magic.Validator.Fail.If(x => x == 0, "A magic number cannot be zero!");
    magic.Validator.Ensure.That(x => x % 10 == 0, x => $"{x} is not a multiple of 10.");

Additionally you can specify a set of attributes for parameters, providing *settings* about that parameter.
The available attributes are:
 - **Default:** Allows you to declare any default value for the parameter, should the user not specify one.
 - **Description:** Allows you to specify a description of the purpose of the parameter.
This information is used when printing the available parameters of a particular function.
 - **IgnoreCase:** For parameters that parse an enumerable type, this attribute specifies that parsing of values should be done without concern for casing.
 - **Name:** Allows you to specify one or several names for a parameter.
These will be required to start with one or more dashes.
If no name is specified, the parameters name will default to the name of the declaring field.
In the above example the parameter would be named `--magic`.
 - **NoName:** Indicates that the parameter does not have a name.
This will typically be used to capture input filenames etc.
Only one unnamed parameter can be specified for a command.
 - **Required:** Lets you define parameters that have one of the following effects; (1) make the command fail if they are not used, (2) prompts the user for a value when the command is executed or (3) prompts the user for a value when the value of the parameter is required by your commands code.

Finally commands also support sub-commands and a flexible structure for defining aliases within your application.