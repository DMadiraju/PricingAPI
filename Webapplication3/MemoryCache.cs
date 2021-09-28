using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing
{
    public class MemoryCache
    {
        [System.Configuration.IntegerValidator(MaxValue = 100, MinValue = 0)]
        public int CacheMemoryLimitMegabytes { get; set; }

        [System.Configuration.IntegerValidator(MaxValue = 100, MinValue = 0)]
        public int PhysicalMemoryLimitPercentage { get; set; }

        public TimeSpan PollingInterval { get; }


        public bool MemoryCaching()
        {
            MemoryCache cache = new MemoryCache();
            cache.PhysicalMemoryLimitPercentage = 0;
            cache.CacheMemoryLimitMegabytes = 10;
            cache.PollingInterval.Milliseconds.ToString("72000");
            return true;
        }


    }
}
