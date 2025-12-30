using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenant.Query.Model.Product
{
    public class PorductFilter
    {
        public bool IncludeReviews { get; set; }
        public bool IncludeCategory { get; set; }
        public bool IncludeImages { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
