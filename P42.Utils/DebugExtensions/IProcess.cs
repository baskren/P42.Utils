namespace P42.Utils;

internal interface IProcess
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    ulong Memory(string caller = "", string callerFile = "", int callerLineNumber = 0);

}
