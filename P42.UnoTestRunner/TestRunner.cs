using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using P42.Utils;

namespace P42.UnoTestRunner;
public class TestRunner
{

    public List<MethodInfo> GetTests()
    {
        foreach (var assembly in P42.Utils.AssemblyExtensions.GetAssemblies())
        {
            foreach (var xType in assembly.GetTypes())
            {
                foreach (var attr in xType.GetCustomAttributes(false))
                {
                    Console.WriteLine($"[{assembly.Name()}:{xType.Name}]:[{attr}]");
                }
            }
        }

        return new List<MethodInfo>();
    }
}
