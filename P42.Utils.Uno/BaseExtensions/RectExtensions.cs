using Windows.Foundation;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class RectExtensions
{

    extension(Rect t1)
    {
        /// <summary>
        /// Increase size of rect by values in Thickness
        /// </summary>
        /// <param name="t2"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public Rect Grow(Thickness t2)
            => new (t1.Left - t2.Left, t1.Top - t2.Top, t1.Width + t2.Left + t2.Right, t1.Height + t2.Top + t2.Bottom);
        
        /// <summary>
        /// Decrease size of rect by values in Thickness
        /// </summary>
        /// <param name="t2"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public Rect Shrink(Thickness t2)
            => new (t1.Left + t2.Left, t1.Top + t2.Top, t1.Width - t2.Left - t2.Right, t1.Height - t2.Top - t2.Bottom);

        /// <summary>
        /// Increase size of rect uniformly
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rect Grow(double value)
            => t1.Grow(new Thickness(value));

        /// <summary>
        /// Decrease size of rect uniformly
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rect Shrink(double value)
            => t1.Shrink(new Thickness(value));

        /// <summary>
        /// Get center X coordinate for rect
        /// </summary>
        /// <returns></returns>
        public double CenterX()
            => (t1.Right + t1.Left)/2.0;

        /// <summary>
        /// Get center Y coordinate for rect
        /// </summary>
        /// <returns></returns>
        public double CenterY()
            => (t1.Bottom + t1.Top)/2.0;
    }
}
