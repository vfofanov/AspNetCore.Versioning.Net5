// Copyright saxu@microsoft.com.  All rights reserved.
// Licensed under the MIT License.

using Microsoft.OData.Edm;

namespace BookStoreAspNetCoreOData8Preview.ODataConfigurations
{
    public interface IODataModelProvider
    {
        IEdmModel GetEdmModel(string apiVersion);
    }
}
