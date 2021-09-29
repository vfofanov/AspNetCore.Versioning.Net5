// Copyright saxu@microsoft.com.  All rights reserved.
// Licensed under the MIT License.

namespace BookStoreAspNetCoreOData8Preview.Models
{
    public abstract class CustomerBase
    {
        public int Id { get; set; }

        public string ApiVersion { get; set; }
    }
}
