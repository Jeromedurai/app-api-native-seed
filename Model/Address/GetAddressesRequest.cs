namespace Tenant.Query.Model.Address
{
    public class GetAddressesRequest
    {
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
        public bool ActiveOnly { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}

