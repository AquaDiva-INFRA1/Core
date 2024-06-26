using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Dcm.UI.Models
{
    public class UserSelectListItemModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public static UserSelectListItemModel Convert(User user)
        {
            return new UserSelectListItemModel()
            {
                Id = user.Id,
                Name = user.Name
            };
        }
    }

    public class UserSelectListModel
    {
        public long Id { get; set; }

        public List<UserSelectListItemModel> UserList { get; set; }

        public UserSelectListModel(string name)
        {
            UserPiManager userPiManager = new UserPiManager();
            UserList = userPiManager.GetPisFromUserByName(name).Select(u => UserSelectListItemModel.Convert(u)).ToList<UserSelectListItemModel>();
            UserList.Count();
        }
    }
}