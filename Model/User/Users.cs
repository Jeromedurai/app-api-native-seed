using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Core;

namespace Tenant.Query.Model.User
{
    public class Users : TnBaseCollection<Model.User.User>
    {
        internal void ApplyFilter(Filter filter)
        {
            bool remove = false;
            this.Items.ToList().ForEach(x =>
            {
                remove = true;
                x.UserRoles.ForEach(ur =>
                {
                    //Checking weather this user is exist in role
                    if (filter.Roles.Contains(ur.Role.RoleName.ToUpper()))
                        remove = false;
                });
                if (remove)
                    this.Items.Remove(x);
            });
        }

        ///// <summary>
        ///// filter the user by role and location
        ///// </summary>
        ///// <param name="locationId">User location id</param>
        ///// <returns></returns>
        //internal List<User> GetApprovalUsers(string locationId)
        //{
        //    //Local variable
        //    List<Model.User.User> userList = new List<User>();

        //    ////filter RM by locaion specific
        //    //var rmUserList = (from user in this.Items
        //    //                  where user.LocationId.Equals(locationId)
        //    //                  from role in user.UserRoles
        //    //                  where Base.Core.Enum.User.Role.RestaurantManager.Equals(System.Enum.Parse(typeof(Base.Core.Enum.User.Role), role.RoleId))
        //    //                  select user).ToList();

        //    var rmUserList = (from user in this.Items
        //                      from role in user.UserRoles
        //                      where API.Base.Core.Enum.User.Role.RestaurantManager.Equals(System.Enum.Parse(typeof(API.Base.Core.Enum.User.Role), role.RoleId))
        //                      from mapping in user.UserLocationMapping
        //                      where mapping.LocationId.Equals(locationId)
        //                      select user).ToList();

        //    //filter all TA and Executive without locaiton 
        //    var fullAccessUserList = (from user in this.Items
        //                              from role in user.UserRoles
        //                              where API.Base.Core.Enum.User.Role.Executive.Equals(System.Enum.Parse(typeof(API.Base.Core.Enum.User.Role), role.RoleId))
        //                              || API.Base.Core.Enum.User.Role.TenantAdmin.Equals(System.Enum.Parse(typeof(API.Base.Core.Enum.User.Role), role.RoleId))
        //                              select user).ToList();
        //    //restaurant user
        //    var ruUserList = (from user in this.Items
        //                      from role in user.UserRoles
        //                      where API.Base.Core.Enum.User.Role.RestaurantUser.Equals(System.Enum.Parse(typeof(API.Base.Core.Enum.User.Role), role.RoleId))
        //                      from mapping in user.UserLocationMapping
        //                      where mapping.LocationId.Equals(locationId)
        //                      select user).ToList();


        //    //Add to collection with distinct record
        //    userList.AddRange(rmUserList);
        //    userList.AddRange(fullAccessUserList);
        //    userList.AddRange(ruUserList);

        //    //userList.AddRange(rmUserList.Except(fullAccessUserList));
        //    //userList.AddRange(fullAccessUserList.Except(rmUserList));
        //    //userList.AddRange(fullAccessUserList.Intersect(rmUserList));

        //    //return user list
        //    return userList.Distinct().ToList();

        //}
    }
}
