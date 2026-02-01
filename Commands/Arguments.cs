namespace ilikefrogs101.CommandHandler;
public class Arguments {
    private static Dictionary<string, string> _arguments = new();

    public void AddArgument(string name, string value) {
        _arguments[name] = value;
    }
    public bool GetArgumentValue<T>(string name, out T value)
    {
        value = default!;
        if (_arguments.TryGetValue(name, out string rawValue))
        {
            try
            {
                value = (T)Convert.ChangeType(rawValue, typeof(T));
                return true;
            }
            catch
            {
                value = default!;
                return false;
            }
        }

        return false;
    }
    public bool FlagTrigged(string flag) {
        return _arguments.ContainsKey(flag);
    }
}