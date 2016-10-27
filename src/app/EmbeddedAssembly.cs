using System;
using System.Collections.Generic;
using System.Reflection;

namespace CDS
{
    public static class EmbeddedAssembly
    {
        private static bool _initialized;
        private static Assembly _currentAssembly;
        private static Assembly _manyConsole;
        private static Assembly _ndeskOptions;
        private static Assembly _cdsCommon, _cdsClient, _cdsServer;

        private static Dictionary<string, Func<Assembly>> _assemblies = new Dictionary<string, Func<Assembly>>
        {
            ["ManyConsole"] = LoadManyConsole,
            ["NDesk.Options"] = LoadNDeskOptions,
            ["CDS.Common"] = LoadCommon,
            ["CDS.Server"] = LoadServer,
            ["CDS.Client"] = LoadClient
        };

        static EmbeddedAssembly()
        {
            InitializeResolver();
        }

        public static void InitializeResolver()
        {
            if (!_initialized)
            {
                AppDomain.CurrentDomain.AssemblyResolve += (x, y) =>
                {
                    var asmName = new AssemblyName(y.Name);
                    //Console.WriteLine($"Resolving [{asmName.Name}]");
                    Func<Assembly> loader;
                    if (!_assemblies.TryGetValue(asmName.Name, out loader))
                    {
                        Console.WriteLine($"Could not find [{asmName.Name}]");
                        return null;
                    }

                    //Console.WriteLine($"Resolved [{asmName.Name}]");
                    return loader();
                };
                _initialized = true;
            }
        }

        private static Assembly LoadEmbeddedAssembly(string resourceName)
        {
            var currentAsm = (_currentAssembly ?? (_currentAssembly = Assembly.GetExecutingAssembly()));
            using (var sr = currentAsm.GetManifestResourceStream(resourceName))
            {
                var data = new byte[sr.Length];
                sr.Read(data, 0, data.Length);
                //return AppDomain.CurrentDomain.Load(data);
                return Assembly.Load(data);
            }
        }

        public static Assembly LoadManyConsole()
        {
            const string resourceName = "CDS.libs.ManyConsole.dll";

            //many console has a dep to ndesk
            LoadNDeskOptions();

            return _manyConsole ?? (_manyConsole = LoadEmbeddedAssembly(resourceName));
        }

        public static Assembly LoadNDeskOptions()
        {
            const string resourceName = "CDS.libs.NDesk.Options.dll";

            return _ndeskOptions ?? (_ndeskOptions = LoadEmbeddedAssembly(resourceName));
        }

        public static Assembly LoadServer()
        {
            LoadCommon();

            const string resourceName = "CDS.libs.CDS.Server.dll";
            return _cdsServer ?? (_cdsServer = LoadEmbeddedAssembly(resourceName));
        }

        public static Assembly LoadClient()
        {
            LoadCommon();

            const string resourceName = "CDS.libs.CDS.Client.dll";
            return _cdsClient ?? (_cdsClient = LoadEmbeddedAssembly(resourceName));
        }

        public static Assembly LoadCommon()
        {
            const string resourceName = "CDS.libs.CDS.Common.dll";
            return _cdsCommon ?? (_cdsCommon = LoadEmbeddedAssembly(resourceName));
        }

    }
}
