using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Dnx.Compilation.Caching;

namespace ConsoleApp1
{
    public class Program
    {
        private readonly IServiceProvider _serviceProvider;

        public Program(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public void Main(string[] args)
        {
            var applicationEnvironment = _serviceProvider.GetService<IApplicationEnvironment>();
            var runtimeEnvironment = _serviceProvider.GetService<IRuntimeEnvironment>();
            var assemblyLoadContextAccessor = _serviceProvider.GetService<IAssemblyLoadContextAccessor>();    

            var moduleContext = new ModuleLoaderContext(
                @"d:\TIEBug\ConsoleApp1\src\ClassLibrary1",
                applicationEnvironment.RuntimeFramework);

            var engine = new CompilationEngine(new CompilationEngineContext(
                applicationEnvironment,
                runtimeEnvironment,
                assemblyLoadContextAccessor.Default,
                new CompilationCache()));

            var exporter = engine.CreateProjectExporter(
                moduleContext.Project, 
                moduleContext.TargetFramework, 
                applicationEnvironment.Configuration);

            // Throws Target Invocation Exception here.
            var exports = exporter.GetExport(moduleContext.Project.Name);

        }
    }

    public class ModuleLoaderContext {
        public ModuleLoaderContext(
            string projectDirectory,
            FrameworkName targetFramework) {

            var applicationHostContext = new ApplicationHostContext {
                ProjectDirectory = projectDirectory,
                TargetFramework = targetFramework
            };

            ApplicationHostContext.Initialize(applicationHostContext);

            LibraryManager = applicationHostContext.LibraryManager;
            Project = applicationHostContext.Project;
            TargetFramework = applicationHostContext.TargetFramework;
        }

        public LibraryManager LibraryManager { get; set; }
        public Project Project { get; set; }
        public FrameworkName TargetFramework { get; private set; }
    }
}
