using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    public class AdminReviewResponse
    {
        public long ReviewId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public bool Verified { get; set; }
        public int Helpful { get; set; }
        public bool Active { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AdminReviewListResponse
    {
        public List<AdminReviewResponse> Reviews { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }

    public class ToggleReviewActiveRequest
    {
        public bool Active { get; set; }
    }
}
