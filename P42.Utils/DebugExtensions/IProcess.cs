namespace P42.Utils;

internal interface IProcess
{
    ulong Memory(string caller = "", string callerFile = "", int callerLineNumber = 0);

}
