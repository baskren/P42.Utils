using System.Reflection;

namespace P42.Utils.Uno;

public static class LocalData
{
    public static LocalDataImageSource Local { get; } = LocalDataImageSource.Instance;
}
