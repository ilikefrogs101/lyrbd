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

        Dictionary<string, string> parsedArguments = new();
        int position = 0;
        for(int i = 0; i < arguments.Length; ++i) {
            string argument = arguments[i];

            if(argument.StartsWith("--")) {
                string flag = argument.Replace("--", "");
                string value = "";

                if(!command.Flags[flag].Boolean) {
                    ++i;
                    value = arguments[i];
                }

                parsedArguments.Add(flag, value);
            }
            else {
                (string name, string value) = _parsePositionalArgument(command, position, arguments[i]);

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
    private static (string, string) _parsePositionalArgument(Command command, int position, string value) {
        if(command.PositionalArguments.Count <= position) return (null, null);

        return (command.PositionalArguments[position].Name, value);
    }
}