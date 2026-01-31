using ilikefrogs101.Logging;
using System.Text.RegularExpressions;

namespace ilikefrogs101.CommandHandler;
public static class CommandParser {
    public static void ParseCommand(string commandRaw) {
        string[] tokens = _tokenise(commandRaw);

        if(tokens.Length == 0) {
            _parsingFailed();
            return;
        }

        string commandName = tokens[0];
        string[] arguments = tokens.Length > 1 ? tokens[1..] : [];
        
        Command command = CommandRegistry.GetCommand(commandName);
        if(command == null) {
            _parsingFailed();
            return;
        }

        Dictionary<string, object> parsedArguments = new();
        int position = 0;
        for(int i = 0; i < arguments.Length; ++i) {
            string argument = arguments[i];

            if(argument.StartsWith("--")) {
                ++i;
                object value = _parseFlag(command, argument[2..], arguments[i]);
                
                if(value == null) {
                    _parsingFailed();
                    return;
                }

                parsedArguments.Add(argument[2..], value);
            }
            else {
                (string name, object value) = _parsePositionalArgument(command, position, arguments[i]);

                if(name == null || value == null) {
                    _parsingFailed();
                    return;
                }

                parsedArguments.Add(name, value);
                ++position;
            }
        }

        command.Execute?.Invoke(parsedArguments);
    }

    private static void _parsingFailed() {
        string helpMenu = "Invalid Command \n\n\n" + CommandRegistry.HelpMenu();
        Log.OutputResponse(helpMenu);
    }
    private static string[] _tokenise(string command) {
        return [.. Regex.Matches(command, @"(?<=^| )(""([^""]|(""""))*""|[^\s""]+)").Select(m => m.Value.Trim('"').Replace("\"\"", "\""))];
    }
    private static object _parseFlag(Command command, string name, string value) {
        if(command.Flags[name] == null) {
            Console.WriteLine($"Unknown Flag {name}");
            return null;
        }

        return Convert.ChangeType(value, command.Flags[name].ValueType);
    }
    private static (string, object) _parsePositionalArgument(Command command, int position, string value) {
        if(command.PositionalArguments.Count <= position) return (null, null);

        return (command.PositionalArguments[position].Name, Convert.ChangeType(value, command.PositionalArguments[position].ValueType));
    }
}