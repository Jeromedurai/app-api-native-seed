using System;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for the response when adding a new category
    /// </summary>
    public class AddCategoryResponse
    {
        /// <summary>
        /// Newly created category ID
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Whether the category is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Parent category ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Order by value
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Category icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Whether the category has sub-menu
        /// </summary>
        public bool SubMenu { get; set; }

        /// <summary>
        /// Category link
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }
    }
}
