using System;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for category list items
    /// </summary>
    public class CategoryListItem
    {
        /// <summary>
        /// Category ID
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
        /// Whether the category has sub-menu
        /// </summary>
        public bool SubMenu { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Order by value
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Parent category ID (0 = no parent)
        /// </summary>
        public long ParentCategoryId { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Menu ID (0 = no menu)
        /// </summary>
        public long MenuId { get; set; }
    }
}
