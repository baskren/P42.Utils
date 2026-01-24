namespace P42.Utils.Uno;

public static class ThicknessExtensions
{
    extension(Thickness thickness)
    {
        /// <summary>
        /// Get total horizontal thickness
        /// </summary>
        /// <returns></returns>
        public double Horizontal()
            => thickness.Left + thickness.Right;

        /// <summary>
        /// Get total vertical thickness
        /// </summary>
        /// <returns></returns>
        public double Vertical()
            => thickness.Top + thickness.Bottom;

        /// <summary>
        /// Get average of Thickness's sides
        /// </summary>
        /// <returns></returns>
        public double Average()
            => (thickness.Horizontal() + thickness.Vertical()) / 4.0;

        /// <summary>
        /// Get maximum value among sides of Thickness
        /// </summary>
        /// <returns></returns>
        public double Max()
            => Math.Max((sbyte)Math.Max(thickness.Left,thickness.Right), (sbyte)Math.Max(thickness.Top,thickness.Bottom));

        /// <summary>
        /// Get minimum value among sides of Thickness
        /// </summary>
        /// <returns></returns>
        public double Min()
            => Math.Min((sbyte)Math.Min(thickness.Left, thickness.Right), (sbyte)Math.Min(thickness.Top, thickness.Bottom));

        /// <summary>
        /// Add two thicknesses
        /// </summary>
        /// <param name="t2"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public Thickness Add(Thickness t2)
            => new (thickness.Left + t2.Left, thickness.Top + t2.Top, thickness.Right + t2.Right, thickness.Bottom + t2.Bottom);

        /// <summary>
        /// Subtract one thickness from another
        /// </summary>
        /// <param name="t2"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public Thickness Subtract(Thickness t2)
            => new (thickness.Left - t2.Left, thickness.Top - t2.Top, thickness.Right - t2.Right, thickness.Bottom - t2.Bottom);

        /// <summary>
        /// Make thickness values negative
        /// </summary>
        /// <returns></returns>
        public Thickness Negate()
            => new (-thickness.Left, -thickness.Top, -thickness.Bottom, -thickness.Right);

        /// <summary>
        /// Uniformly add to thickness
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Thickness Add(double offset)
            => thickness.Add(new Thickness(offset));

        /// <summary>
        /// Uniformly subtract from thickness
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Thickness Subtract(double offset)
            => thickness.Subtract(new Thickness(offset));
    }


}
