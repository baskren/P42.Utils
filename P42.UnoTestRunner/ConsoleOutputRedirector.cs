using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

public class ConsoleOutputRedirector : IDisposable
{
    private TextWriterRedirector? _duplicator;

    private Lock _lock= new Lock();
    private bool disposedValue;

    public ConsoleOutputRedirector()
    {
    }

    public event EventHandler<string>? ContentChanged;

    private void OnContentChanged(object? sender, string e)
        => ContentChanged?.Invoke(null, e);
    

    public void Start()
    {
        lock (_lock)
        {
            if (_duplicator != null) return;

            _duplicator = new TextWriterRedirector(Console.Out);
            _duplicator.ContentChanged += OnContentChanged;
            Console.SetOut(_duplicator);
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_duplicator == null) return;

            Console.SetOut(_duplicator.InnerWriter);
            _duplicator.ContentChanged -= OnContentChanged;
            _duplicator?.Dispose();
            _duplicator = null;
        }
    }



    public string GetContentAndReset()
        => _duplicator?.GetContentAndReset() ?? "";

    public string Content => _duplicator?.Content ?? "";

    public void Reset() => _duplicator?.Reset();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                Stop();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

internal partial class TextWriterRedirector(TextWriter inner) : TextWriter
{
    public readonly TextWriter InnerWriter = inner;
    private readonly StringBuilder _accumulator = new();
    public bool _paused;

    public void Start() => _paused = false;

    public void Stop() => _paused = true;

    public event EventHandler<string>? ContentChanged;

    internal string GetContentAndReset()
    {
        var result = _accumulator.ToString();
        Reset();
        System.Diagnostics.Debug.WriteLine(result);
        return result;
    }

    public string Content => _accumulator.ToString();

    internal void Reset() => _accumulator.Clear();

    public override Encoding Encoding { get; } = inner.Encoding;


    public override void Write(char value)
    {
        if (_paused) 
            InnerWriter.Write(value);
        else
        {
            _accumulator.Append(value);
            ContentChanged?.Invoke(this, Content);
        }
    }

}
