using System.Globalization;

namespace NachoPrompt
{
    /// <summary>
    /// This class holds the main functionality of NPrompt.
    /// </summary>
    /// <remarks>
    /// <para>
    /// NPrompt is a C# Rewrite of QPrompt by Jeff Rimko.
    /// <see href="https://github.com/jeffrimko/Qprompt"/>
    /// </para>
    /// </remarks>
    public class NPrompt
    {

        /// <summary>
        /// The current library version number.
        /// </summary>
        public static readonly string Version = "0.1.0";
        private static string userChar = ": ";
        private static char findChar = '/';
        private static bool echoReturn = true;

        /// <summary>
        /// A queue for CLI arguments.
        /// </summary>
        private static readonly Queue<string> ArgumentQueue = new();
        /// <summary>
        /// Prompt start character sequence
        /// </summary>
        public static string StartChar { get; set; } = "[?] ";
        /// <summary>
        /// User input start character sequence
        /// </summary>
        public static string UserChar { get => userChar; set => userChar = value; }
        /// <summary>
        /// Default horizontal rule width
        /// </summary>
        public static int HRWidth { get; set; } = 65;
        /// <summary>
        /// Default horizontal rule character
        /// </summary>
        public static char HRChar { get; set; } = '-';
        /// <summary>
        /// Default menu find/search command character.
        /// </summary>
        public static char FindChar { get => findChar; set => findChar = value; }

        /// <summary>
        /// If true, prevents <see cref="Console.Write(string?)"/> from displaying
        /// </summary>
        public static bool Silent { get; set; } = false;
        /// <summary>
        /// If true, <see cref="Echo(string, string)"/> returns the string to print.
        /// </summary>
        public static bool EchoReturn { get => echoReturn; set => echoReturn = value; }



        // Console Output
        /// <summary>
        /// Clears the console and resets any lingering colors
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
            Console.ResetColor();
        }

        /// <summary>
        /// Prints a message to console
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <param name="end">The newline string. Default is "\n"</param>
        public static void Echo(string msg = "", string end = "\n")
        {
            Console.Write(msg + end);
        }
        /// <summary>
        /// Prints alert messages to console
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <returns>Printed string</returns>
        public static string Alert(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[!] ");
            Console.ResetColor();
            Console.WriteLine(msg);
            return msg;
        }
        /// <summary>
        /// Prints info message to console
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <returns>Printed message</returns>
        public static string Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[INFO] ");
            Console.ResetColor();
            Console.WriteLine(msg);
            return msg;
        }
        /// <summary>
        /// Prints warning message to console
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <returns>Printed message</returns>
        public static string Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[WARNING] ");
            Console.ResetColor();
            Console.WriteLine(msg);
            return msg;
        }
        /// <summary>
        /// Prints error message to console
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <returns>Printed message</returns>
        public static string Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ResetColor();
            Console.WriteLine(msg);
            return msg;
        }

        /// <summary>
        /// Waits for a user console input before moving on
        /// </summary>
        public static void Pause()
        {
            Console.ReadKey(true);
        }

        /// <summary>
        /// Draw a horizontal rule
        /// </summary>
        /// <param name="width">The width of the rule. Default is <see cref="HRWidth"/></param>
        /// <param name="rule_char">The character used in the rule. Default is <see cref="HRChar"/></param>
        public static void HorizontalRule(int? width = null, char? rule_char = null)
        {
            width ??= HRWidth;
            rule_char ??= HRChar;
            for (int i = 0; i < width.Value; i++)
            {
                Console.Write(rule_char);
            }
            Console.Write('\n');
        }
        /// <summary>
        /// Wrap a specified message to a specific width
        /// </summary>
        /// <param name="msg">The message to wrap</param>
        /// <param name="width">The width to wrap. Default is <see cref="HRWidth"/></param>
        /// <param name="horizontalRules">Should the message be wrapped by horizontal rules. Defaults to <c>true</c></param>

        public static void Wrap(string msg, int? width = null, bool horizontalRules = true)
        {
            width ??= HRWidth;
            if (horizontalRules)
                HorizontalRule(width);
            int n = 0;
            bool lastNL = false;
            for (int i = 0; i < msg.Length; i++)
            {
                lastNL = false;
                if (msg[i] == '\n')
                {
                    n = 0;
                    Console.Write('\n');
                    lastNL = true;
                    continue;
                }
                Console.Write(msg[i]);
                n++;
                if (n == width)
                {
                    n = 0;
                    Console.Write('\n');
                    lastNL = true;
                }
            }
            if (!lastNL)
                Console.Write('\n');
            if (horizontalRules)
                HorizontalRule(width);
        }

        // User Input
        /// <summary>
        /// Add the specified arguments to the argument queue.
        /// </summary>
        /// <param name="args">Arguments to be automatically used</param>
        public static void QueueArguments(params string[] args)
        {
            foreach (string arg in args)
            {
                ArgumentQueue.Enqueue(arg);
            }
        }


        private static string WritePrompt(string msg, string? def = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(StartChar);
            Console.ResetColor();
            Console.Write(msg);
            if (def != "")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{def}]");
                Console.ResetColor();
            }
            Console.Write(UserChar);
            return (StartChar + msg + UserChar);
        }

        private static string GetInputFromDict<T>(Dictionary<string, T> valids)
        {
            string output = "";
            foreach (string key in valids.Keys)
            {
                output += $" [{key}]";
            }
            output = output.TrimStart();
            return output;
        }
        /// <summary>
        /// A generic ask method.
        /// </summary>
        /// <typeparam name="T">The output type of the response</typeparam>
        /// <param name="msg">The message to prompt in console</param>
        /// <param name="def">The default response</param>
        /// <param name="valids">A <see cref="Dictionary{TKey, TValue}"/> where the TKey is a <see cref="string"/> input, and the TValue is the output of type T</param>
        /// <param name="caseSensitive">If set to false, will lowercase the input to ignore all uppercase</param>
        /// <param name="allowBlanks">If false, will reject black inputs when there is no default</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>The formated output</returns>
        public static T Ask<T>(string msg = "Enter input", string def = "", Dictionary<string, T>? valids = null, bool caseSensitive = false, bool allowBlanks = false, bool allowAuto = true)
        {
            if (def != "") WritePrompt(msg, def);
            else WritePrompt(msg);

            string? ans;
            if (ArgumentQueue.Count != 0 && allowAuto)
            {
                ans = ArgumentQueue.Dequeue();
            }
            else
            {
                ans = Console.ReadLine() ?? "";
            }

            if (ans == "")
            {
                ans = def;
            }
            if (ans == "" && !allowBlanks)
            {
                if (valids == null)
                {
                    Error($"Incorrect format. Input must be {typeof(T)}");
                }
                else
                {
                    Error("Invalid input. Input must be one of the following: " + GetInputFromDict<T>(valids));
                }
                return Ask<T>(msg, def, valids, caseSensitive, allowBlanks, allowAuto: allowAuto);
            }

            if (!caseSensitive) ans = ans.ToLower();

            if (valids != null)
            {
                if (valids.TryGetValue(ans, out T? value))
                {
                    return value;
                }
                else if (valids.TryGetValue("\0", out value))
                {
                    return value;
                }
                else
                {
                    Error("Invalid input. Input must be one of the following: " + GetInputFromDict<T>(valids));
                    return Ask<T>(msg, def, valids, caseSensitive, allowBlanks, allowAuto: allowAuto);
                }
            }
            else
            {

                try
                {
                    T output = (T)Convert.ChangeType(ans, typeof(T));
                    return output;
                }
                catch (FormatException)
                {
                    Error($"Incorrect format. Input must be {typeof(T)}");
                    return Ask<T>(msg, def, valids, caseSensitive, allowBlanks, allowAuto: allowAuto);
                }
                catch (Exception e)
                {
                    Error(e.Message);
                    return Ask<T>(msg, def, valids, caseSensitive, allowBlanks, allowAuto: allowAuto);
                }
            }

        }

        /// <summary>
        /// Prompt for a yes or no
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="def">Default response</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>True if "y" or "yes", and False if "n" or "no"</returns>
        public static bool AskYesNo(string msg = "Proceed?", bool? def = null, bool allowAuto = true)
        {
            Dictionary<string, bool> yesnodict = new()
            {
                ["y"] = true,
                ["yes"] = true,
                ["n"] = false,
                ["no"] = false
            };

            if (def != null)
            {
                bool defb = (bool)def;
                return Ask<bool>(msg, (defb ? "YES" : "NO"), yesnodict, allowAuto: allowAuto);
            }
            return Ask<bool>(msg, "", yesnodict, allowBlanks: false, allowAuto: allowAuto);
        }

        /// <summary>
        /// Prompt for an integer
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="def">Default response</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>An <see cref="int"/></returns>
        public static int AskInt(string msg = "Enter an integer", int? def = null, int? minimum = null, int? maximum = null, bool allowAuto = true)
        {
            int num;
            num = Ask<int>(msg, def.ToString() ?? "", allowAuto: allowAuto);

            if (minimum != null && num < minimum)
            {
                Error($"Must be a number equal to or greater than {minimum}");
                return AskInt(msg, def, minimum, maximum);
            }

            if (maximum != null && num > minimum)
            {
                Error($"Must be a number equal to or less than {maximum}");
                return AskInt(msg, def, minimum, maximum);
            }

            return num;
        }


        /// <summary>
        /// Prompt for a float
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="def">Default response</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>A <see cref="float"/></returns>
        public static float AskFloat(string msg = "Enter a float", float? def = null, int? minimum = null, int? maximum = null, bool allowAuto = true)
        {
            float num;
            num = Ask<float>(msg, def.ToString() ?? "", allowAuto: allowAuto);

            if (minimum != null && num < minimum)
            {
                Error($"Must be a number equal to or greater than {minimum}");
                return AskFloat(msg, def, minimum, maximum);
            }

            if (maximum != null && num > minimum)
            {
                Error($"Must be a number equal to or less than {maximum}");
                return AskFloat(msg, def, minimum, maximum);
            }

            return num;
        }


        /// <summary>
        /// Prompt for a string
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="def">Default response</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>A <see cref="string"/></returns>
        public static string AskString(string msg = "Enter a string", string def = "", bool allowAuto = true)
        {
            if (def != "")
            {
                return Ask<string>(msg, def, caseSensitive: true, allowBlanks: true, allowAuto: allowAuto);
            }
            return Ask<string>(msg, caseSensitive: true, allowBlanks: true, allowAuto: allowAuto);
        }
        /// <summary>
        /// Prompt for a password
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="hiddenChar">Character to display in place of typed key</param>
        /// <param name="displayHiddenChar">Should we display any characters at all</param>
        /// <param name="allowAuto">Accepts items in the <see cref="ArgumentQueue"/> as input</param>
        /// <returns>The password in <see cref="string"/> format</returns>
        public static string AskPass(string msg = "Enter a password", char hiddenChar = '*', bool displayHiddenChar = true, bool allowAuto = true)
        {
            WritePrompt(msg);

            string pass = "";
            if (ArgumentQueue.Count == 0 && allowAuto)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                while (key.KeyChar != '\r')
                {
                    if (key.KeyChar == '\b')
                    {
                        if (pass.Length == 0)
                        {
                            key = Console.ReadKey(true);
                            continue;
                        }
                        // range operator
                        pass = pass[..^1];

                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;

                        key = Console.ReadKey(true);
                        continue;
                    }
                    pass += key.KeyChar;
                    if (displayHiddenChar) Console.Write(hiddenChar);
                    key = Console.ReadKey(true);
                }
            }
            else
            {
                pass = ArgumentQueue.Dequeue();
            }
            Console.Write("\n");
            return pass;
        }

        private static string GenerateCaptureString(Random rand, int length, bool complex = false)
        {
            string output = "";
            char randomChar;
            for (int i = 0; i < length; i++)
            {
                if (complex)
                {
                    randomChar = (char)rand.Next(33, 127);
                }
                else
                {
                    int type = rand.Next(3);
                    int offset = 33;
                    int key = 0;
                    switch (type)
                    {
                        case 0:
                            offset = 48;
                            key = rand.Next(10);
                            break;
                        case 1:
                            offset = 65;
                            key = rand.Next(26);
                            break;
                        case 2:
                            offset = 97;
                            key = rand.Next(26);
                            break;
                    }
                    randomChar = (char)(offset + key);
                }
                output += randomChar;
            }
            return output;
        }
        /// <summary>
        /// Prompt for a capture to weed out bots
        /// </summary>
        /// <param name="msg">Prompt message</param>
        /// <param name="length">The length of the capture. Default is <c>6</c></param>
        /// <param name="random">An instance of <see cref="Random"/>. Default is <see cref="Random.Shared"/></param>
        /// <returns></returns>
        public static bool AskCapture(string msg = "Enter the capture", int length = 6, Random? random = null)
        {
            random ??= Random.Shared;
            string capture = GenerateCaptureString(random, length);
            Dictionary<string, bool> validDict = new()
            {
                { capture, true }, // Our specified capture
                { "\0", false } // Literally anything else
            };
            return Ask<bool>($"{msg} [{capture}]", caseSensitive: true, valids: validDict, allowAuto: false);
        }

        /// <summary>
        /// A class for easily creating CLI menus
        /// </summary>
        public class Menu
        {
            /// <summary>
            /// The header of the menu. Can be set with <see cref="SetHeader(string)"/>.
            /// </summary>
            public string Header = string.Empty;
            private readonly Dictionary<string, string> options = new();
            private readonly Dictionary<string, Action> optionActions = new();

            /// <summary>
            /// Maximum results to display in a menu before making it page based.
            /// </summary>
            /// <remarks>
            /// Not currently implimented
            /// </remarks>
            public int MaxResults;

            /// <summary>
            /// Should you clear the console before showing the menu
            /// </summary>
            public bool ClearOnShow;

            /// <summary>
            /// Initializes the menu with the default header "MENU"
            /// </summary>
            public Menu()
            {
                Initialize("MENU");
            }
            /// <summary>
            /// Initializes the menu with the specified <paramref name="header"/>
            /// </summary>
            /// <param name="header">Header of the menu</param>
            public Menu(string header)
            {
                Initialize(header);
            }

            /// <summary>
            /// Initializes the menu with the specified <paramref name="header"/>.
            /// The menu is then populated by <paramref name="args"/>
            /// </summary>
            /// <param name="header">Header for the menu</param>
            /// <param name="args">An array of strings to be added to the menu</param>
            public Menu(string header, params string[] args)
            {
                Initialize(header);
                foreach (string arg in args)
                {
                    Add(arg, arg);
                }
            }
            public Menu(params Action[] args)
            {
                Initialize("MENU");
                foreach (Action arg in args)
                {
                    Add(arg.Method.Name, arg.Method.Name, arg);
                }
            }

            private void Initialize(string header)
            {
                Header = header;
                MaxResults = 20;
                ClearOnShow = false;
            }

            public Menu SetClearOnShow(bool clear)
            {
                ClearOnShow = clear;
                return this;
            }

            public Menu SetMaxResults(int n)
            {
                MaxResults = n;
                return this;
            }

            public Menu SetHeader(string header)
            {
                Header = header;
                return this;
            }

            public IEnumerable<string> GetOptions()
            {
                foreach (string option in options.Keys)
                {
                    yield return option;
                }
            }

            public bool HasOption(string option)
            {
                return options.ContainsKey(option);
            }

            public bool HasAction(string option)
            {
                return optionActions.ContainsKey(option);
            }
            public Menu Add(string key, string value, Action? action = null)
            {
                options[key] = value;
                if (action != null)
                    optionActions[key] = action;
                return this;
            }

            public Menu Add(string key, Action? action = null)
            {
                options[key] = key;
                if (action != null)
                    optionActions[key] = action;
                return this;
            }

            public Menu Remove(string key)
            {
                options.Remove(key);
                return this;
            }

            public Menu CopyTo(out Menu menu)
            {
                menu = new Menu();
                foreach (string option in GetOptions())
                {
                    optionActions.TryGetValue(option, out Action? action);
                    menu.Add(option, options[option], action);
                }
                return this;
            }

            public string Show(string msg = "Enter menu selection")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"-- {Header} --");

                foreach (string option in GetOptions())
                {
                    string value = options[option];
                    if (option == value)
                        Console.Write($"   {value}\n");
                    else
                        Console.Write($"   ({option}) {value}\n");
                }
                while (true)
                {
                    string input = "";
                    WritePrompt(msg);
                    if (ArgumentQueue.Count != 0)
                    {
                        input = ArgumentQueue.Dequeue();
                    }
                    else
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        while (key.KeyChar != '\r')
                        {
                            if (key.KeyChar == '\b')
                            {
                                if (input.Length == 0)
                                {
                                    key = Console.ReadKey(true);
                                    continue;
                                }
                                input = input[..^1];

                                Console.CursorLeft--;
                                Console.Write(" ");
                                Console.CursorLeft--;

                                key = Console.ReadKey(true);
                                continue;
                            }

                            switch (char.GetUnicodeCategory(key.KeyChar))
                            {
                                case UnicodeCategory.Control:
                                case UnicodeCategory.Format:
                                    key = Console.ReadKey(true);
                                    continue;
                                default:
                                    break;
                            }

                            input += key.KeyChar;
                            Console.Write(key.KeyChar);
                            key = Console.ReadKey(true);
                        }
                    }
                    Console.Write('\n');


                    if (HasOption(input))
                    {
                        if (HasAction(input))
                        {
                            optionActions[input].Invoke();
                        }
                        return input;
                    }
                    Error("Invalid input.");
                }
            }
        }

    }
}