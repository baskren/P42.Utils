using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.UI.Dispatching;

namespace P42.Utils.Uno
{
	public static class Platform
	{
        public static double MinFontSize = 10.0;

        public static Microsoft.UI.Xaml.Application Application { get; private set; }

        public static Microsoft.UI.Xaml.Window Window { get; private set; }

        public static Thread MainThread { get; private set; }

        public static DispatcherQueue MainThreadDispatchQueue { get; private set; }

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

        static Assembly _assembly;
        public static Assembly Assembly => _assembly ??= typeof(Platform).Assembly;

        static Microsoft.UI.Xaml.Media.FontFamily _mathFontFamily;
        public static Microsoft.UI.Xaml.Media.FontFamily MathFontFamily => _mathFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/STIXGeneral.ttf#STIXGeneral");

        static Microsoft.UI.Xaml.Media.FontFamily _sansSerifFontFamily;
        public static Microsoft.UI.Xaml.Media.FontFamily SansSerifFontFamily => _sansSerifFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI#Regular");

        static Microsoft.UI.Xaml.Media.FontFamily _monoSpaceFontFamily;
        public static Microsoft.UI.Xaml.Media.FontFamily MonoSpaceFontFamily => _monoSpaceFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code");


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
            MainThread = Thread.CurrentThread;
            MainThreadDispatchQueue = DispatcherQueue.GetForCurrentThread();

            P42.NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.BeginInvokeOnMainThread;
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
