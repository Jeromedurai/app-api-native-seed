using System.Collections.Generic;

namespace Tenant.Query.Model.Response.MenuMaster
{
    public class MenuMaster
    {
        public int menuId { get; set; }
        public string menuName { get; set; }
        public int orderBy { get; set; }
        public bool active { get; set; }
        public bool subMenu { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public List<Category> category { get; set; }
        public string id { get; set; }
    }

    public class Category
    {
        public int categoryId { get; set; }
        public string category { get; set; }
        public bool active { get; set; }
    }
}
