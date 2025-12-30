using System.ComponentModel;

namespace Tenant.Query.Model
{
    public class Enum
    {
        public enum ReturnStatus
        {
            [Description("Non-returnable")]
            Non_Returnable = 1,
            [Description("Returnable")]
            Returnable = 2,
        }

        public enum Trending
        {
            [Description("Sale")]
            Sale = 1,
            [Description("New")]
            New= 2,
            [Description("Trending")]
            Trending= 3,
        }

        public enum ActionType
        {
            [Description("Approve")]
            APPROVE,
            [Description("Reject")]
            REJECT
        }
    }
}
