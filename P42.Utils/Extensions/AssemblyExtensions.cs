using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace P42.Utils
{
    public static class AssemblyExtensions
    {
        public static bool EmbeddedResourceExists(this Assembly assembly, string resourceId)
        {
            foreach (var res in assembly.GetManifestResourceNames())
                if (res == resourceId)
                    return true;
            return false;
        }

        public static void TryCopyResource(this Assembly assembly, string resourceId, string path)
        {
            if (assembly.GetManifestResourceStream(resourceId) is Stream stream)
            {
                WriteToFile(stream, path);
                stream.Dispose();
            }
        }


        public static void WriteToFile(Stream stream, string destinationFile, int bufferSize = 4096, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
        {
            using (var destinationFileStream = new FileStream(destinationFile, mode, access, share))
            {
                while (stream.Position < stream.Length)
                {
                    destinationFileStream.WriteByte((byte)stream.ReadByte());
                }
            }
        }

        public static Assembly FindAssemblyForMultiResource(string resourceId, Assembly assembly = null)
        {
            if (assembly?.GetManifestResourceNames().Any(id => id.StartsWith(resourceId, StringComparison.Ordinal)) ?? false)
                return assembly;
            if (resourceId.IndexOf(".Resources.", StringComparison.Ordinal) is int index && index > 0)
            {
                var assemblyName = resourceId.Substring(0, index);
                assembly = GetAssemblyByName(assemblyName);
                if (assembly?.GetManifestResourceNames().Any(id => id.StartsWith(resourceId, StringComparison.Ordinal)) ?? false)
                    return assembly;
            }
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly?.GetManifestResourceNames().Any(id => id.StartsWith(resourceId, StringComparison.Ordinal)) ?? false)
                return assembly;
            foreach (var assm in ReflectionExtensions.GetAssemblies())
            {
                if (!assm.IsDynamic && assm.GetManifestResourceNames().Any(id => id.StartsWith(resourceId, StringComparison.Ordinal)))
                    return assm;
            }
            return null;
        }

        public static Assembly GetAssemblyByName(string name)
        {
            foreach (Assembly assembly in GetAssemblies())
            {
                if (assembly.GetName().Name == name)
                {
                    return assembly;
                }
            }

            return null;
        }

        public static List<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()?.ToList();
        }

    }
}
