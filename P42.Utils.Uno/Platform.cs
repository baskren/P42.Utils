﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno
{
	public static class Platform
	{
        public static double MinFontSize = 10.0;

        public static Microsoft.UI.Xaml.Application Application { get; private set; }

        public static Microsoft.UI.Xaml.Window Window { get; private set; }

        static List<Assembly> _assembliesToInclude;
        public static List<Assembly> AssembliesToInclude
        {
            get
            {
                if (_assembliesToInclude == null)
                {
                    _assembliesToInclude = new List<Assembly>
                    {
                        typeof(P42.Utils.DebugExtensions).Assembly
                    };
                }
                return _assembliesToInclude;
            }
        }

        public static void Init(Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.Window window)
		{
			Application = application;
            Window = window;
			P42.Utils.Environment.Init();
            P42.Utils.Environment.PlatformTimer = new Timer();
            P42.Utils.Environment.PlatformPathLoader = PlatformPathLoader;
			P42.Utils.DiskSpace.PlatformDiskSpace = new DiskSpace();
			P42.Utils.Process.PlatformProcess = new P42.Utils.Uno.Process();
            P42.Utils.Environment.EmbeddedResourceAssemblyResolver = EmbeddedResourceExtensions.FindAssemblyForResourceId;

        }

		static void PlatformPathLoader()
		{
			/*
            //var envVars = System.Environment.GetEnvironmentVariables();

            try
            {
                //var documentsFolderPath = Windows.Storage.KnownFolders.DocumentsLibrary?.Path;
                //P42.Utils.Environment.DocumentsPath = documentsFolderPath;//Windows.Storage.ApplicationData.Current.LocalFolder.Path; 
            }
            catch (System.UnauthorizedAccessException)
            {
                // we don't have access.  Oh well.
            }
            */
			P42.Utils.Environment.ApplicationDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
			P42.Utils.Environment.ApplicationCachePath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
			P42.Utils.Environment.TemporaryStoragePath = Windows.Storage.ApplicationData.Current.TemporaryFolder.Path;
		}
        /*
        public static Assembly AssemblyFromResourceId(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return null;
            Assembly assembly = null;
            var resourcePath = new List<string>(resourceId.Split('.'));
            if (resourceId.Contains(".Resources."))
            {
                var resourceIndex = resourcePath.IndexOf("Resources");
                var asmName = string.Join(".", resourcePath.GetRange(0, resourceIndex));
                var appAsm = Microsoft.UI.Xaml.Application.Current.GetType().Assembly;  
                if (appAsm?.GetName().Name == asmName)
                    return appAsm;

                foreach (var asm in AssembliesToInclude)
                    if (asm?.GetName().Name == asmName)
                        return asm;
            }
            if (assembly == null)
            {
                for (int i = resourcePath.Count - 1; i < 0; i--)
                {
                    var asmName = string.Join(".", resourcePath.GetRange(0, i));
                    foreach (var asm in Platform.AssembliesToInclude)
                        if (asm?.GetName().Name == asmName)
                            return asm;
                }
            }
            return assembly;
        }
        */

    }
}
