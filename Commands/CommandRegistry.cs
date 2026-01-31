using System.Reflection;
using System.Text;

namespace ilikefrogs101.CommandHandler;
public static class CommandRegistry {
    private static Dictionary<string, Command> _commands = new();
    public static void LoadCommands(Type type) {

        if (!type.IsAbstract || !type.IsSealed) return;

        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        for(int i = 0; i < methods.Length; ++i) {
            MethodInfo method = methods[i];

            CommandAttribute commandAttribute = method.GetCustomAttribute<CommandAttribute>();
            if(commandAttribute == null) continue;

            Command command = new Command {
                Name = commandAttribute.Name,
                Description = commandAttribute.Description,
                Execute = (Dictionary<string, string> arguments) => method.Invoke(null, [arguments])
            };

            IEnumerable<ArgumentAttribute> arguments = method.GetCustomAttributes<ArgumentAttribute>();
            foreach(ArgumentAttribute argument in arguments) {
                if(argument.ArgumentType == ArgumentType.Flag) {
                    command.Flags[argument.Name] = argument;
                }
                else {
                    command.PositionalArguments.Add(argument);
                }
            }

            _commands.Add(command.Name, command);
        }
    }
    public static Command GetCommand(string name) {
        if(!_commands.TryGetValue(name, out Command command)) return null;

        return command;
    }
    public static string HelpMenu() {
        StringBuilder menu = new();

        for(int i = 0; i < _commands.Count; ++i) {
            Command command = _commands.ElementAt(i).Value;
            menu.Append(command.Name);
            menu.Append(": ");
            menu.Append(command.Description);
            menu.Append("\n  Usage: ");
            menu.Append(command.Name);
            for(int j = 0; j < command.PositionalArguments.Count; ++j) {
                if(command.PositionalArguments[j].ArgumentType == ArgumentType.PositionalOptional) {
                    menu.Append(" ?");
                }
                else {
                    menu.Append(" [");
                }

                menu.Append(command.PositionalArguments[j].Name);

                if(command.PositionalArguments[j].ArgumentType == ArgumentType.PositionalOptional) {
                    menu.Append('?');
                }
                else {
                    menu.Append(']');
                }           
            }
            for(int j = 0; j < command.Flags.Count; ++j) {
                menu.Append(" --");
                menu.Append(command.Flags.ElementAt(j).Value.Name);
                
                if(!command.Flags.ElementAt(j).Value.Boolean)
                    menu.Append(" [value]");
            } 
            menu.Append("\n\n");
        }

        return menu.ToString();
    }
}