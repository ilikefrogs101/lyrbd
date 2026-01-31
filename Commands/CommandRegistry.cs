using System.Reflection;

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
                Execute = (Dictionary<string, object> arguments) => method.Invoke(null, new object[]{arguments})
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
}