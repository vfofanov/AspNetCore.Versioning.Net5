using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Versioning
{
    public class DefaultVersioningRoutingPrefixProvider : IVersioningRoutingPrefixProvider
    {
        private readonly string _prefixFormat;

        public DefaultVersioningRoutingPrefixProvider(string prefixFormat = "{0}")
        {
            if (string.IsNullOrWhiteSpace(prefixFormat))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(prefixFormat));
            }

            _prefixFormat = prefixFormat;
        }

        /// <inheritdoc />
        public virtual string GetPrefix(ControllerModel controller, ApiVersionInfo version)
        {
            return VersioningRoutingPrefixHelper.GeneratePrefix(_prefixFormat, version);
        }
    }
}