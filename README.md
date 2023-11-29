# NPrompt
![Static Badge](https://img.shields.io/badge/License-Apache_2.0-green?link=https%3A%2F%2Fgithub.com%2FTheGreatNacho%2FNPrompt%2Fblob%2Fmain%2FLICENSE.txt) ![Static Badge](https://img.shields.io/badge/Current_Version-v1.0.0-orange)
> NPrompt: A Lightweight C# Command-Line Interface Library for User Interaction

## Introduction
NPrompt, a product inspired by innovative Python libraries like [Qprompt](https://github.com/jeffrimko/Qprompt "Qprompt Github"), brings simplicity to C# CLI interactions. It provides an efficient solution for creating dynamic CLI prompts and handling user input. Key features of NPrompt include:
* Straightforward multi-entry menus
* Type-specific prompts (integer, float, string)
* Support for optional default values
* Integration with script command-line arguments for automation

## System Requirements
NPrompt is crafted using .NET 7.0, ensuring a seamless experience across platforms.

## Examples
NPrompt is simple to use.
```csharp
bool IsAlive = NPrompt.AskYesNo("Are you alive?");

if (IsAlive)
{
	int Age = NPrompt.AskInt("How old are you?");
	if (Age > 18)
	{
		Console.WriteLine("Hello World!");
	}
}

# Output:
# [?] Are you alive?: Yes
# [?] How old are you?: 30
# Where the user inputs Yes and 30.
```
You can also do menus!
```csharp
var Menu = new NPrompt.Menu("Hello World!");
Menu.Add("h", "Hello World Again");
Menu.Add("q", "Quit");

var result = Menu.Show();

# Output:
# -- Hello World! --
#    (h) Hello World Again
#    (q) Quit
# [?] Enter menu selection:
```

## Get Involved
Your contributions and feedback are vital to the evolution of NPrompt. Feel free to participate and help shape the future of command-line interfaces in C#!

## Contribution Guidelines
Contributions and feedback are always welcome and encouraged!
