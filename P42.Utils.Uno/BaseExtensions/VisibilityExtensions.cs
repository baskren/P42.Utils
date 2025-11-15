namespace P42.Utils.Uno;

public static class VisibilityExtensions
{
    /// <summary>
    /// Returns opposite visibility as given
    /// </summary>
    /// <param name="visibility"></param>
    /// <returns></returns>
    public static Visibility Inverse(this Visibility visibility)
        =>visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    
}
