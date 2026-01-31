namespace ilikefrogs101.CommandHandler;
[AttributeUsage(AttributeTargets.Field)]
public class ArgumentAttribute : Attribute{
    public string Name;
    public string Usage;
    public Type ValueType;
    public ArgumentType ArgumentType;
}
public enum ArgumentType {
    PositionalRequired,
    PositionalOptional,
    Flag
}