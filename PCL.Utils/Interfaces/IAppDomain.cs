// /*******************************************************************
//  *
//  * AppDomain.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/
using System.Collections.Generic;
using System.Reflection;

namespace PCL.Utils
{
	public interface IAppDomain
	{
		IList<IAssembly> GetAssemblies();

		Assembly GetAssemblyByName(string name);
	}

	public interface IAssembly
	{
		string Name { get; }
		Assembly Assembly { get; }
	}


	public class AppDomainWrapper
	{
		public static IAppDomain Instance { get; set; }
	}
}

