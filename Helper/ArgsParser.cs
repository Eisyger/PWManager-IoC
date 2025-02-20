namespace PWManager;

public static class ArgsParser
{
    public static bool Register(string[] args)
    {
        return args.Any(x => x == "-register");
    }
}