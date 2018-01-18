// /*******************************************************************
//  *
//  * AppDomain.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/
using System.Collections.Generic;
using System.Reflection;
using System;

namespace P42.Utils
{
    public interface IAppDomain
    {
        IList<Assembly> GetAssemblies();

        Assembly GetAssemblyByName(string name);

        IEnumerable<Type> GetChildClassesOf(Type parentType);
    }


    public class AppDomainWrapper
    {
        public static IAppDomain Instance { get; set; }

        public static IList<Assembly> GetAssemblies()
        {
            return Instance?.GetAssemblies();
        }

        public static Assembly GetAssemblyByName(string name)
        {
            return Instance?.GetAssemblyByName(name);
        }

        public static IEnumerable<Type> GetChildClassesOf(Type parentType)
        {
            return Instance?.GetChildClassesOf(parentType);
        }
    }
}

