// /*******************************************************************
//  *
//  * AppDomain.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if __IOS__
namespace PCL.Utils.iOS
#elif __DROID__
namespace PCL.Utils.Droid
#else
namespace PCL.Utils
#endif
//namespace PCL.Utils.iOS
{
    public class AppDomainWrapperInstance : IAppDomain
    {
        IList<Assembly> IAppDomain.GetAssemblies()
        {
            return GetAssemblies();
        }

        IList<Assembly> GetAssemblies()
        {
            /*
            var result = new List<Assembly>();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                //result.Add(new AssemblyWrapper(assembly));
                result.Add(assembly);
            return result;
            */
            return AppDomain.CurrentDomain.GetAssemblies();
        }



        Assembly IAppDomain.GetAssemblyByName(string name)
        {
            return GetAssemblyByName(name);
        }

        Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }



        IEnumerable<Type> IAppDomain.GetChildClassesOf(Type parentType)
        {
            return GetChildClassesOf(parentType);
        }

        IEnumerable<Type> GetChildClassesOf(Type parentType)
        {
            foreach (var asm in GetAssemblies())
                foreach (var type in asm.GetTypes())
                    if (type.IsSubclassOf(parentType))
                        yield return type;
        }

    }

    /*
	public class AssemblyWrapper : IAssembly
	{
		readonly Assembly _Assembly;
		public AssemblyWrapper(Assembly assembly)
		{
			_Assembly = assembly;
		}

		public string Name { get { return _Assembly.GetName().ToString(); } }

		public Assembly Assembly { get { return _Assembly; } }

	}
	*/

}

