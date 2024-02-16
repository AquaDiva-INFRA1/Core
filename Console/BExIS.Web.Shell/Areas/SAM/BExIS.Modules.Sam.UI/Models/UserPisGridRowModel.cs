using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BExIS.Web.Shell.Areas.SAM.Models
{
    public class UserPisGridRowModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public long PiId { get; set; }

        public string PiName { get; set; }


        public static UserPisGridRowModel Convert(UserPi userPi)
        {
            SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);

            return new UserPisGridRowModel()
            {
                Id = userPi.Id,
                UserId = userPi.UserId,
                PiId = userPi.PiId,
                UserName = MappingUser.Name,
                PiName = MappingPi.Name
            };
        }
    }

    public class UserPiEditModel
    {
        public long UserId { get; set; }

        public long Id { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        public long PiId { get; set; }

        public string PiName { get; set; }

        public List<User> Pis { get; set; }

        public List<User> UserList { get; set; }

        public UserPi Pi { get; set; }

        public User CurrentPi { get; set; }

        public string SelectedPiName { get; set; }

        public UserPiEditModel(long userPiId)
        {
            UserPiManager userPiManager = new UserPiManager();
            UserPi userPi = userPiManager.GetUserPiById(userPiId);

            SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);

            Id = userPi.Id;
            UserId = userPi.UserId;
            PiId = userPi.PiId;
            UserName = MappingUser.Name;
            PiName = MappingPi.Name;
            SelectedPiName = MappingPi.Name;
            CurrentPi = MappingPi;

            UserList = new SubjectManager().GetAllUsers().ToList().OrderBy(u => u.Name).ToList();
        }

        public UserPiEditModel()
        {
            UserList = new SubjectManager().GetAllUsers().ToList().OrderBy(u => u.Name).ToList();
        }

        private List<UserPi> Convert(List<User> UserList)
        {
            List<UserPi> userPiList = new List<UserPi>();

            foreach (User user in UserList)
            {
                //long piId = (new UserPiManager().GetPisFromUserById(user.Id)).First().Id;
                UserPi newUserPi = new UserPi();
                //userPiList.Add(newUserPi);
            }

            return userPiList;
        }
        /*
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Email]
        public string Email { get; set; }

        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Blocked")]
        public bool IsBlocked { get; set; }

        [Display(Name = "Locked Out")]
        public bool IsLockedOut { get; set; }
         * */



        public static UserPiEditModel Convert(UserPi userPi)
        {


            SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);

            return new UserPiEditModel()
            {
                Id = userPi.Id,
                UserId = userPi.UserId,
                PiId = userPi.PiId,
                UserName = MappingUser.Name,
                PiName = MappingPi.Name
            };
        }


        /*
       public class UserPiSelectListItemModel
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
         * */
    }

    public class UserPiCreateModel
    {
        public List<User> UserList;
        public List<User> PiUserList;
        public string SelectedUserName { get; set; }
        public string SelectedPiName { get; set; }

        public UserPiCreateModel()
        {
            SubjectManager Sum = new SubjectManager();
            this.UserList = Sum.GetAllUsers().OrderBy(u => u.Name).ToList();
            this.PiUserList = Sum.GetAllUsers().OrderBy(u => u.Name).ToList();
        }
    }
}
