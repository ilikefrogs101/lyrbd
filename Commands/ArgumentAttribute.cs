namespace ilikefrogs101.CommandHandler;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ArgumentAttribute : Attribute{
    public string Name;
    public Type ValueType;
    public ArgumentType ArgumentType;
}
public enum ArgumentType {
    PositionalRequired,
    PositionalOptional,
    Flag
}