namespace P42.Utils.Uno;

/// <summary>
/// Garbage Collection Extensions
/// </summary>
public static class GC
{
    /// <summary>
    /// Collect Garbage (including Java.Lang.JavaSystem.Gc)
    /// </summary>
    public static void Collect()
    {
        System.GC.Collect();
#if __ANDROID__
            Java.Lang.JavaSystem.Gc();
#endif
    }
}
