namespace P42.Utils
{
    interface IProcess
    {
        ulong Memory(string caller = "", string callerFile = "", int callerLineNumber = 0);

    }
}
