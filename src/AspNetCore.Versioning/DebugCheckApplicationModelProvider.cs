using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Versioning
{
    public class DebugCheckApplicationModelProvider : IApplicationModelProvider
    {
        /// <inheritdoc />
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
        }

        /// <inheritdoc />
        public int Order => -1000;
    }
}