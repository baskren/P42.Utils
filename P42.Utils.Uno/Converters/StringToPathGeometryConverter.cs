using System;
using System.Globalization;
using Windows.Foundation;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno;

public class StringToPathGeometryConverter : IValueConverter
{
    public static StringToPathGeometryConverter Current { get; } = new();

    #region Const & Private Variables

    private const bool AllowSign = true;
    private const bool AllowComma = true;
    private const bool IsFilled = true;
    private const bool IsClosed = true;

    private IFormatProvider?    _formatProvider;

    private PathFigure?         _figure;                        // Figure object, which will accept parsed segments
    private string              _pathString = string.Empty;     // Input string to be parsed
    private int                 _pathLength;
    private int                 _curIndex;                      // Location to read next character from

    private Point               _lastStart;                     // Last figure starting point
    private Point               _lastPoint;                     // Last point 
    private Point               _secondLastPoint;               // The point before last point

    private char                _token;                         // Non whitespace character returned by ReadToken
    #endregion

    private StringToPathGeometryConverter() { }

    #region Public Functionality
    /// <summary>
    /// Main conversion routine - converts string path data definition to PathGeometry object
    /// </summary>
    /// <param name="path">String with path data definition</param>
    /// <returns>PathGeometry object created from string definition</returns>
    public PathGeometry Convert(string path)
        => string.IsNullOrWhiteSpace(path) 
            ? new PathGeometry() 
            : Parse(path);
    

    /// <summary>
    /// Main back conversion routine - converts PathGeometry object to its string equivalent
    /// </summary>
    /// <param name="geometry">Path Geometry object</param>
    /// <returns>String equivalent to PathGeometry contents</returns>
    public string ConvertBack(PathGeometry geometry)
    {
        if (null == geometry)
            throw new ArgumentException("Path Geometry cannot be null!");

        return string.Empty;
    }
    #endregion

    #region Private Functionality
    /// <summary>
    /// Main parser routine, which loops over each char in received string, and performs actions according to command/parameter being passed
    /// </summary>
    /// <param name="path">String with path data definition</param>
    /// <returns>PathGeometry object created from string definition</returns>
    private PathGeometry Parse(string path)
    {
        PathGeometry? pathGeometry = null;
        
        _formatProvider = CultureInfo.InvariantCulture;
        _pathString = path;
        _pathLength = path.Length;
        _curIndex = 0;

        _secondLastPoint = new Point(0, 0);
        _lastPoint = new Point(0, 0);
        _lastStart = new Point(0, 0);

        var first = true;

        var lastCmd = ' ';

        while (ReadToken()) // Empty path is allowed in XAML
        {
            var cmd = _token;

            if (first)
            {
                if (cmd != 'M' && cmd != 'm' && cmd != 'f' && cmd != 'F')  // Path starts with M|m 
                    ThrowBadToken();

                first = false;
            }

            switch (cmd)
            {
                case 'f':
                case 'F':
                    pathGeometry = new PathGeometry();
                    var num = ReadNumber(!AllowComma);
                    pathGeometry.FillRule = num == 0 ? FillRule.EvenOdd : FillRule.Nonzero;
                    break;

                case 'm':
                case 'M':
                    // XAML allows multiple points after M/m
                    _lastPoint = ReadPoint(cmd, !AllowComma);
                    EnsureFigure();
                    _lastStart = _lastPoint;
                    lastCmd = 'M';

                    while (IsNumber(AllowComma))
                    {
                        _lastPoint = ReadPoint(cmd, !AllowComma);

                        var lineSegment = new LineSegment { Point = _lastPoint };
                        _figure!.Segments.Add(lineSegment);
                        //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                        lastCmd = 'L';
                    }
                    break;

                case 'l':
                case 'L':
                case 'h':
                case 'H':
                case 'v':
                case 'V':
                    EnsureFigure();

                    do
                    {
                        switch (cmd)
                        {
                            case 'l':
                            case 'L':
                                _lastPoint = ReadPoint(cmd, !AllowComma); break;
                            case 'h': _lastPoint.X += ReadNumber(!AllowComma); break;
                            case 'H': _lastPoint.X = ReadNumber(!AllowComma); break;
                            case 'v': _lastPoint.Y += ReadNumber(!AllowComma); break;
                            case 'V': _lastPoint.Y = ReadNumber(!AllowComma); break;
                        }

                        var lineSegment = new LineSegment { Point = _lastPoint };
                        _figure!.Segments.Add(lineSegment);
                        //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                    }
                    while (IsNumber(AllowComma));

                    lastCmd = 'L';
                    break;

                case 'c':
                case 'C': // cubic Bezier 
                case 's':
                case 'S': // smooth cubic Bezier
                    EnsureFigure();

                    do
                    {
                        Point p;

                        if (cmd is 's' or 'S')
                        {
                            p = lastCmd == 'C' ? Reflect() : _lastPoint;
                            _secondLastPoint = ReadPoint(cmd, !AllowComma);
                        }
                        else
                        {
                            p = ReadPoint(cmd, !AllowComma);

                            _secondLastPoint = ReadPoint(cmd, AllowComma);
                        }

                        _lastPoint = ReadPoint(cmd, AllowComma);
                        var bezierSegment = new BezierSegment
                        {
                            Point1 = p,
                            Point2 = _secondLastPoint,
                            Point3 = _lastPoint
                        };
                        _figure!.Segments.Add(bezierSegment);
                        //context.BezierTo(p, _secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);

                        lastCmd = 'C';
                    }
                    while (IsNumber(AllowComma));

                    break;

                case 'q':
                case 'Q': // quadratic Bezier 
                case 't':
                case 'T': // smooth quadratic Bezier
                    EnsureFigure();

                    do
                    {
                        if (cmd is 't' or 'T')
                        {
                            _secondLastPoint = lastCmd == 'Q' ? Reflect() : _lastPoint;
                            _lastPoint = ReadPoint(cmd, !AllowComma);
                        }
                        else
                        {
                            _secondLastPoint = ReadPoint(cmd, !AllowComma);
                            _lastPoint = ReadPoint(cmd, AllowComma);
                        }

                        var quadraticBezierSegment = new QuadraticBezierSegment
                        {
                            Point1 = _secondLastPoint,
                            Point2 = _lastPoint
                        };
                        _figure!.Segments.Add(quadraticBezierSegment);
                        //context.QuadraticBezierTo(_secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);
                        lastCmd = 'Q';
                    }
                    while (IsNumber(AllowComma));

                    break;

                case 'a':
                case 'A':
                    EnsureFigure();

                    do
                    {
                        // A 3,4 5, 0, 0, 6,7
                        var w = ReadNumber(!AllowComma);
                        var h = ReadNumber(AllowComma);
                        var rotation = ReadNumber(AllowComma);
                        var large = ReadBool();
                        var sweep = ReadBool();

                        _lastPoint = ReadPoint(cmd, AllowComma);

                        var arcSegment = new ArcSegment
                        {
                            Point = _lastPoint,
                            Size = new Size(w, h),
                            RotationAngle = rotation,
                            IsLargeArc = large,
                            SweepDirection = sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
                        };
                        _figure!.Segments.Add(arcSegment);
                        //context.ArcTo(
                        //    _lastPoint,
                        //    new Size(w, h),
                        //    rotation,
                        //    large,
                        //    sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                        //    IsStroked,
                        //    !IsSmoothJoin
                        //    );
                    }
                    while (IsNumber(AllowComma));

                    lastCmd = 'A';
                    break;

                case 'z':
                case 'Z':
                    EnsureFigure().IsClosed = IsClosed;
                    //context.SetClosedState(IsClosed);
                    lastCmd = 'Z';
                    _lastPoint = _lastStart; // Set reference point to be first point of current figure
                    break;

                default:
                    ThrowBadToken();
                    break;
            }

            if (_figure is not { IsClosed: true })
                continue;

            pathGeometry ??= new PathGeometry();
            pathGeometry.Figures.Add(_figure);
            _figure = null;
            first = true;

        }

        pathGeometry ??= new PathGeometry();
        if (_figure != null && !pathGeometry.Figures.Contains(_figure))
            pathGeometry.Figures.Add(_figure);
        
        return pathGeometry;
    }

    private void SkipDigits(bool signAllowed)
    {
        // Allow for a sign 
        if (signAllowed && More() && (_pathString[_curIndex] == '-' || _pathString[_curIndex] == '+'))
        {
            _curIndex++;
        }

        while (More() && _pathString[_curIndex] >= '0' && _pathString[_curIndex] <= '9')
        {
            _curIndex++;
        }
    }

    private bool ReadBool()
    {
        SkipWhiteSpace(AllowComma);

        if (More())
        {
            _token = _pathString[_curIndex++];

            if (_token == '0')
            {
                return false;
            }

            if (_token == '1')
            {
                return true;
            }
        }

        ThrowBadToken();

        return false;
    } 

    private Point Reflect()
    {
        return new Point(2 * _lastPoint.X - _secondLastPoint.X,
            2 * _lastPoint.Y - _secondLastPoint.Y);
    }

    private PathFigure EnsureFigure()
        => _figure ??= new PathFigure
        {
            StartPoint = _lastPoint,
            IsFilled = IsFilled,
            IsClosed = !IsClosed
        };
    

    private double ReadNumber(bool allowComma)
    {
        if (!IsNumber(allowComma))
        {
            ThrowBadToken();
        }

        var simple = true;
        var start = _curIndex;

        //
        // Allow for a sign
        //
        // There are numbers that cannot be preceded with a sign, for instance, -NaN, but it's 
        // fine to ignore that at this point, since the CLR parser will catch this later.
        // 
        if (More() && (_pathString[_curIndex] == '-' || _pathString[_curIndex] == '+'))
        {
            _curIndex++;
        }

        // Check for Infinity (or -Infinity).
        if (More() && _pathString[_curIndex] == 'I')
        {
            // 
            // Don't bother reading the characters, as the CLR parser will 
            // do this for us later.
            // 
            _curIndex = Math.Min(_curIndex + 8, _pathLength); // "Infinity" has 8 characters
            simple = false;
        }
        // Check for NaN 
        else if (More() && _pathString[_curIndex] == 'N')
        {
            // 
            // Don't bother reading the characters, as the CLR parser will
            // do this for us later. 
            //
            _curIndex = Math.Min(_curIndex + 3, _pathLength); // "NaN" has 3 characters
            simple = false;
        }
        else
        {
            SkipDigits(!AllowSign);

            // Optional period, followed by more digits 
            if (More() && _pathString[_curIndex] == '.')
            {
                simple = false;
                _curIndex++;
                SkipDigits(!AllowSign);
            }

            // Exponent
            if (More() && (_pathString[_curIndex] == 'E' || _pathString[_curIndex] == 'e'))
            {
                simple = false;
                _curIndex++;
                SkipDigits(AllowSign);
            }
        }

        if (simple && _curIndex <= start + 8) // 32-bit integer
        {
            var sign = 1;

            if (_pathString[start] == '+')
            {
                start++;
            }
            else if (_pathString[start] == '-')
            {
                start++;
                sign = -1;
            }

            var value = 0;

            while (start < _curIndex)
            {
                value = value * 10 + (_pathString[start] - '0');
                start++;
            }

            return value * sign;
        }

        var subString = _pathString.Substring(start, _curIndex - start);

        try
        {
            return System.Convert.ToDouble(subString, _formatProvider);
        }
        catch (FormatException except)
        {
            throw new FormatException($"Unexpected character in path '{_pathString}' at position {_curIndex - 1}", except);
        }
    } 

    private bool IsNumber(bool allowComma)
    {
        var commaMet = SkipWhiteSpace(allowComma);

        if (More())
        {
            _token = _pathString[_curIndex];

            // Valid start of a number
            // I: infinity
            // N: not-a-number
            if (_token is '.' or '-' or '+' or 'I' or 'N') 
                return true;
            if (_token is >= '0' and <= '9')
                return true;
        }

        if (commaMet) // Only allowed between numbers
            ThrowBadToken();

        return false;
    }

    private Point ReadPoint(char cmd, bool allowComma)
    {
        var x = ReadNumber(allowComma);
        var y = ReadNumber(AllowComma);

        if (cmd < 'a') // 'A' < 'a'. lower case for relative
            return new Point(x, y);

        x += _lastPoint.X;
        y += _lastPoint.Y;
        return new Point(x, y);
    } 

    private bool ReadToken()
    {
        SkipWhiteSpace(!AllowComma);
        if (!More())
            return false;

        _token = _pathString[_curIndex++];
        return true;
    }

    private bool More()
        => _curIndex < _pathLength;
    

    // Skip white space, one comma if allowed
    private bool SkipWhiteSpace(bool allowComma)
    {
        var commaMet = false;

        while (More())
        {
            var ch = _pathString[_curIndex];

            switch (ch)
            {
                case ' ':
                case '\n':
                case '\r':
                case '\t': // SVG whitespace 
                    break;

                case ',':
                    if (allowComma)
                    {
                        commaMet = true;
                        allowComma = false; // one comma only
                    }
                    else
                        ThrowBadToken();

                    break;

                default:
                    // Avoid calling IsWhiteSpace for ch in (' ' ... 'z')
                    if ( ch is > ' ' and <= 'z' || !char.IsWhiteSpace(ch) )
                        return commaMet;

                    break;
            }

            _curIndex++;
        }

        return commaMet;
    }

    private void ThrowBadToken()
        => throw new FormatException($"Unexpected character in path '{_pathString}' at position {_curIndex - 1}");
    

    internal static char GetNumericListSeparator(IFormatProvider provider)
    {
        var numericSeparator = ',';

        // Get the NumberFormatInfo out of the provider, if possible.
        // If the IFormatProvider doesn't contain a NumberFormatInfo, then 
        // this method returns the current culture's NumberFormatInfo. 
        var numberFormat = NumberFormatInfo.GetInstance(provider);

        // Is the decimal separator is the same as the list separator?
        // If so, we use the ";". 
        if (numberFormat.NumberDecimalSeparator.Length > 0 && numericSeparator == numberFormat.NumberDecimalSeparator[0])
            numericSeparator = ';';

        return numericSeparator;
    }

    /*
    private string parseBack(PathGeometry geometry)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        IFormatProvider provider = new System.Globalization.CultureInfo("en-us");
        string format = null;

        sb.Append("F" + (geometry.FillRule == FillRule.EvenOdd ? "0" : "1") + " ");

        foreach (PathFigure figure in geometry.Figures)
        {
            sb.Append("M " + ((IFormattable)figure.StartPoint).ToString(format, provider) + " ");

            foreach(PathSegment segment in figure.Segments)
            {
                char separator = GetNumericListSeparator(provider);

                if (segment.GetType() == typeof(LineSegment))
                {
                    LineSegment _lineSegment = segment as LineSegment;

                    sb.Append("L " + ((IFormattable)_lineSegment.Point).ToString(format, provider) + " ");
                }
                else if (segment.GetType() == typeof(BezierSegment))
                {
                    BezierSegment _bezierSegment = segment as BezierSegment;

                    sb.Append(String.Format(provider,
                         "C{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "} ",
                         separator,
                         _bezierSegment.Point1,
                         _bezierSegment.Point2,
                         _bezierSegment.Point3
                         ));
                }
                else if (segment.GetType() == typeof(QuadraticBezierSegment))
                {
                    QuadraticBezierSegment _quadraticBezierSegment = segment as QuadraticBezierSegment;

                    sb.Append(String.Format(provider,
                         "Q{1:" + format + "}{0}{2:" + format + "} ",
                         separator,
                         _quadraticBezierSegment.Point1,
                         _quadraticBezierSegment.Point2));
                }
                else if (segment.GetType() == typeof(ArcSegment))
                {
                    ArcSegment _arcSegment = segment as ArcSegment;

                    sb.Append(String.Format(provider,
                         "A{1:" + format + "}{0}{2:" + format + "}{0}{3}{0}{4}{0}{5:" + format + "} ",
                         separator,
                         _arcSegment.Size,
                         _arcSegment.RotationAngle,
                         _arcSegment.IsLargeArc ? "1" : "0",
                         _arcSegment.SweepDirection == SweepDirection.Clockwise ? "1" : "0",
                         _arcSegment.Point));
                }
            }

            if (figure.IsClosed)
                sb.Append("Z");
        }

        return sb.ToString();
    }
    */
    #endregion

    #region IValueConverter Members
    // ReSharper disable UnusedParameter.Global

    public object Convert(object value)
    {
        if (value is not string path)
            path = string.Empty;
        return Convert(path);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => Convert(value);
    
    public object ConvertBack(object value)
    {
        if (value is not PathGeometry geometry)
            geometry = new PathGeometry();

        return ConvertBack(geometry);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => ConvertBack(value);

    public object Convert(object value, Type targetType, object parameter, string language)
        => Convert(value);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => ConvertBack(value);

    // ReSharper restore UnusedParameter.Global
    #endregion
}
