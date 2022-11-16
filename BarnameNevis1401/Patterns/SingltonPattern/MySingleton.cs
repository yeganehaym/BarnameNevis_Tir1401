namespace BarnameNevis1401.Patterns.SingltonPattern;

public class MySingleton
{
    public string Value { get; set; }
}

public static class MySingletonCreator
{
    private static MySingleton _mySingleton;

    public static MySingleton CreateInstance()
    {
        if (_mySingleton == null)
            _mySingleton = new();
        return _mySingleton;
    }
}