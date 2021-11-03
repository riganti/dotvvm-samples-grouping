using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridViewGrouping.Helpers
{
    public record GroupInfo
    {
        
        public string GroupKey { get; set; }

        public string GroupDisplayName { get; set; }
        
        public bool IsFirstEntryInGroup { get; set; }

        public bool IsExpanded { get; set; }

    }
}
