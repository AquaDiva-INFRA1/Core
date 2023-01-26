using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.Sam.UI.Models
{
    public class UserPisGridRowModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public long PiId { get; set; }

        public string PiName { get; set; }


        public UserPisGridRowModel Convert(UserPi userPi)
        {
            /*SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);*/
            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            User MappingPi = repo.Query( u => u.Id.Equals(userPi.PiId) ).FirstOrDefault();
            User MappingUser = repo.Query(u => u.Id.Equals(userPi.UserId)).FirstOrDefault();

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

            /*SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);*/
            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            User MappingPi = repo.Query(u => u.Id.Equals(userPi.PiId)).FirstOrDefault();
            User MappingUser = repo.Query(u => u.Id.Equals(userPi.UserId)).FirstOrDefault();

            Id = userPi.Id;
            UserId = userPi.UserId;
            PiId = userPi.PiId;
            UserName = MappingUser.Name;
            PiName = MappingPi.Name;
            SelectedPiName = MappingPi.Name;
            CurrentPi = MappingPi;

            UserList = repo.Get().ToList().OrderBy(u => u.Name).ToList();
        }

        public UserPiEditModel()
        {
            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            UserList = repo.Get().ToList().OrderBy(u => u.Name).ToList();
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



        public UserPiEditModel Convert(UserPi userPi)
        {


            /*SubjectManager sum = new SubjectManager();
            User MappingPi = sum.GetUserById(userPi.PiId);
            User MappingUser = sum.GetUserById(userPi.UserId);*/
            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            User MappingPi = repo.Query(u => u.Id.Equals(userPi.PiId)).FirstOrDefault();
            User MappingUser = repo.Query(u => u.Id.Equals(userPi.UserId)).FirstOrDefault();

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
            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            this.UserList = repo.Get().OrderBy(u => u.Name).ToList();
            this.PiUserList = repo.Get().OrderBy(u => u.Name).ToList();
        }
    }
}
