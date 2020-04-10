using System;
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
            if (args.Length < 2)
            {
                Console.WriteLine("Args required: [Assembly] [Directory]");
                Console.WriteLine("Please provide an assembly to look for...");
                Console.WriteLine("Please provide a valid path to a bin directory...");
                
                return;
            }


            string assembly = args[0];
            string filePath = args[1];
            //string filePath = "C:/git/gridstreamansiadapter/gsansiadapterserver/bin/debug/net462/win81-x64";


            if (Directory.Exists(filePath))
            {
                string[] files = Directory.GetFiles(filePath, "*.dll");


                foreach (string file in files)
                {
                    bool load = true;
                    Assembly a = null;

                    try
                    {
                        a = Assembly.LoadFile(file);
                        load = true;
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Couldn't load assembly");
                        //Console.WriteLine(e);
                        load = false;
                    }

                    if (load)
                    {
                        AssemblyName[] dependencies = a.GetReferencedAssemblies();

                        foreach (AssemblyName dependency in dependencies)
                        {
                            if (dependency.Name.ToLower() == assembly.ToLower())
                            {
                                Console.WriteLine("{0} has a dependency on {1}, version {2}", file, assembly, dependency.Version);
                            }
                        }
                    }

                    
                    
                }
            }
            else
            {
                Console.WriteLine("{0} is not a directory...");
            }
            
        }
    }
}
