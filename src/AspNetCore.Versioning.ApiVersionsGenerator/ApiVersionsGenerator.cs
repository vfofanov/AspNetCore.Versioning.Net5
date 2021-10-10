//Uncomment for debug
//#define DEBUG_GENERATION

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Versioning.ApiVersionsGenerator
{
        [Generator]
        public class ApiVersionsGenerator : ISourceGenerator
        {
#if DEBUG && DEBUG_GENERATION
        private const bool launchDebugger = true;
#else
            private const bool launchDebugger = false;
#endif

            public virtual void Initialize(GeneratorInitializationContext context)
            {
                if (launchDebugger)
                {
                    if (!Debugger.IsAttached)
                    {
                        Debugger.Launch();
                    }    
                }
            }

            /// <inheritdoc />
            public void Execute(GeneratorExecutionContext context)
            {
            }
        }
}