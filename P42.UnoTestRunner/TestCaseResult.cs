using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

public record TestCaseResult
{
    public TestResult TestResult { get; init; }
    public string? TestName { get; init; }
    public TimeSpan Duration { get; init; }
    public string? Message { get; init; }
}
