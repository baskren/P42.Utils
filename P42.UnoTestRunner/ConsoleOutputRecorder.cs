using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

internal partial class ConsoleOutputRecorder : IDisposable
{
    private readonly TextWriterDuplicator _duplicator;
    private readonly TextWriter _originalOutput;

    private int _isDisposed;

    public static ConsoleOutputRecorder Start()
        => new();

    private ConsoleOutputRecorder()
    {
        _originalOutput = Console.Out;
        _duplicator = new TextWriterDuplicator(_originalOutput);

        Console.SetOut(_duplicator);
    }

    internal string GetContentAndReset()
        => _duplicator.GetContentAndReset();

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            GC.SuppressFinalize(this);
            Console.SetOut(_originalOutput);
        }
    }

    ~ConsoleOutputRecorder()
    {
        Dispose();
    }
}

internal partial class TextWriterDuplicator(TextWriter inner) : TextWriter
{
    private readonly TextWriter _inner = inner;
    private readonly StringBuilder _accumulator = new();

    internal string GetContentAndReset()
    {
        var result = _accumulator.ToString();
        Reset();
        System.Diagnostics.Debug.WriteLine(result);
        return result;
    }

    internal void Reset() => _accumulator.Clear();

    public override Encoding Encoding { get; } = inner.Encoding;

    public override void Write(char value)
    {
        _inner.Write(value);
        _accumulator.Append(value);
    }
}
