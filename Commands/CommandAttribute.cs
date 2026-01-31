namespace ilikefrogs101.CommandHandler;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute {
    public string Name;
    public string Description;
}