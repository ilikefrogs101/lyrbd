namespace ilikefrogs101.CommandHandler;
public class Command {
    public string Name;
    public string Description;
    public List<ArgumentAttribute> PositionalArguments = new();
    public Dictionary<string, ArgumentAttribute> Flags = new();
    public Action<Arguments> Execute;
}