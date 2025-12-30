using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tenant.Query.Model.Authentication
{
    public class ValidTokenContext
    {
        [Column("ISVALID"), Key]
        public bool IsValid { get; set; }
    }
}
