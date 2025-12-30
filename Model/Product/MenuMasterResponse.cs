using System;
using System.Collections.Generic;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Model for menu master response
    /// </summary>
    public class MenuMasterResponse
    {
        /// <summary>
        /// List of menu master items
        /// </summary>
        public List<MenuMasterItem> MenuMaster { get; set; } = new List<MenuMasterItem>();
    }

    /// <summary>
    /// Model for individual menu master item
    /// </summary>
    public class MenuMasterItem
    {
        /// <summary>
        /// Menu ID
        /// </summary>
        public long MenuId { get; set; }

        /// <summary>
        /// Menu name
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// Order by value
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Whether the menu is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Menu image
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Whether the menu has sub-menu
        /// </summary>
        public bool SubMenu { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Categories associated with this menu
        /// </summary>
        public List<MenuCategoryItem> Category { get; set; } = new List<MenuCategoryItem>();
    }

    /// <summary>
    /// Model for category items within menu
    /// </summary>
    public class MenuCategoryItem
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
        /// Category order
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Category icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }
    }
}
