using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AssemblyScanner
{
    class Program
    {
        // arg 1, dependency to scan for
        // arg 2, directory to scan
        static void Main(string[] args)
        {
            if (!ValidateArgs(args))
            {                
                return;
            }

            string assembly = args[0];
            string filePath = args[1];

            Dictionary<string, List<string>> versionDictionary = new Dictionary<string, List<string>>();

            if (Directory.Exists(filePath))
            {
                string[] files = Directory.GetFiles(filePath, "*.dll");

                foreach (string file in files)
                {
                    Assembly a = TryLoadAssembly(file);

                    if (a != null)
                    {
                        AssemblyName[] dependencies = a.GetReferencedAssemblies();

                        foreach (AssemblyName dependency in dependencies)
                        {
                            CompareAssembly(assembly, versionDictionary, file, dependency);
                        }
                    }
                }

                DumpResults(assembly, versionDictionary);
            }
            else
            {
                Console.WriteLine("{0} is not a directory...");
            }
        }

        private static bool ValidateArgs(string[] args)
        {
            bool valid = true;
            if (args.Length < 2)
            {
                Console.WriteLine("Args required: [Assembly] [Directory]");
                Console.WriteLine("Please provide an assembly to look for...");
                Console.WriteLine("Please provide a valid path to a bin directory...");

                valid = false;
            }

            return valid;
        }

        private static Assembly TryLoadAssembly(string file)
        {
            Assembly a = null;
            try
            {
                a = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                // If we weren't able to load, it is likely due to a mismatched assembly architecture
                // Just ignore it...
            }

            return a;
        }

        private static void CompareAssembly(string assembly, Dictionary<string, List<string>> versionDictionary, string file, AssemblyName dependency)
        {
            if (dependency.Name.ToLower() == assembly.ToLower())
            {
                FileInfo fi = new FileInfo(file);

                List<string> versions = null;
                if (versionDictionary.TryGetValue(dependency.Version.ToString(), out versions))
                {
                    versions.Add(fi.Name);
                }
                else
                {
                    List<string> f = new List<string>();
                    f.Add(fi.Name);
                    versionDictionary.Add(dependency.Version.ToString(), f);
                }
            }
        }

        private static void DumpResults(string assembly, Dictionary<string, List<string>> versionDictionary)
        {
            if (versionDictionary.Count > 1)
            {
                foreach (string key in versionDictionary.Keys)
                {
                    List<string> filesWithDependencyVersion = null;

                    versionDictionary.TryGetValue(key, out filesWithDependencyVersion);

                    Console.WriteLine("Version {0} of {1}", key, assembly);
                    Console.WriteLine("---------------------");


                    foreach (string file in filesWithDependencyVersion)
                    {
                        Console.WriteLine(file);
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
            else if (versionDictionary.Count == 0)
            {
                Console.WriteLine("No assemblies are using {0}", assembly);
            }
            else
            {
                Console.WriteLine("All assemblies are using the same version of {0}", assembly);
            }
        }
    }
}
