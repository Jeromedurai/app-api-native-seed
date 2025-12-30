using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for updating an existing category
    /// </summary>
    public class UpdateCategoryRequest
    {
        /// <summary>
        /// Category ID (required)
        /// </summary>
        [Required(ErrorMessage = "Category ID is required")]
        public long CategoryId { get; set; }

        /// <summary>
        /// Category name (required)
        /// </summary>
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(255, ErrorMessage = "Category name cannot exceed 255 characters")]
        public string CategoryName { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the category is active (default: true)
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Parent category ID (optional)
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// Order by value (default: 0)
        /// </summary>
        public int OrderBy { get; set; } = 0;

        /// <summary>
        /// Category icon
        /// </summary>
        [StringLength(255, ErrorMessage = "Icon cannot exceed 255 characters")]
        public string Icon { get; set; }

        /// <summary>
        /// Whether the category has sub-menu (default: false)
        /// </summary>
        public bool HasSubMenu { get; set; } = false;

        /// <summary>
        /// Category link
        /// </summary>
        [StringLength(500, ErrorMessage = "Link cannot exceed 500 characters")]
        public string Link { get; set; }

        /// <summary>
        /// Menu ID (optional)
        /// </summary>
        public long? MenuId { get; set; }
    }
}
