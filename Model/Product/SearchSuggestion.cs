using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    public class SearchSuggestionRequest
    {
        public long TenantId { get; set; }
        public string Query { get; set; } = string.Empty;
        public int Limit { get; set; } = 8;
    }

    public class SearchSuggestion
    {
        public string SuggestionText { get; set; } = string.Empty;
        public string SuggestionType { get; set; } = string.Empty; // "product", "category", "keyword", "recent"
        public long? ProductId { get; set; }
        public long? CategoryId { get; set; }
        public int MatchScore { get; set; }
        public decimal? Price { get; set; }
    }

    public class SearchSuggestionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SearchSuggestion> Suggestions { get; set; } = new List<SearchSuggestion>();
        public string Query { get; set; } = string.Empty;
    }

    // Enhanced search request with new filter options
    public class EnhancedProductSearchPayload : ProductSearchPayload
    {
        public int? NewArrivalsDays { get; set; }  // 7 or 30
        public bool? Trending { get; set; }
        public int TrendingThreshold { get; set; } = 10;
        public bool FuzzySearch { get; set; } = false;
    }
}
