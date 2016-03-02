using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using BExIS.Web.Shell.Areas.SAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using DataAnnotationsExtensions;

namespace BExIS.Web.Shell.Areas.SAM.Controllers
{
    public class UserPisController : Controller
    {
        //
        // GET: /SAM/UserPis/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserPis()
        {
            return View();
        }

        [GridAction]
        public ActionResult UserPis_Select()
        {
            List<UserPisGridRowModel> users = new List<UserPisGridRowModel>();
            UserPiManager userPiManager = new UserPiManager();

            ICollection<UserPi> collection = userPiManager.GetAllUserPis();

            if (collection != null)
            {
                users = userPiManager.GetAllUserPis().Select(upis => UserPisGridRowModel.Convert(upis)).ToList();
            }

            return View(new GridModel<UserPisGridRowModel> { Data = users });
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            UserPiManager userPiManager = new UserPiManager();

            UserPiEditModel model = new UserPiEditModel(id);

            model.Pi = userPiManager.GetUserPiById(id);

            return PartialView("_EditPartial", model);
        }

        [HttpPost]
        public ActionResult Edit(UserPiEditModel userPiEditModel)
        {


            //User currentPi = userPiEditModel.CurrentPi;
            string newPiName = userPiEditModel.SelectedPiName;
            User newPi = new SubjectManager().GetUserByName(newPiName);
            long user = userPiEditModel.UserId;
            long entryId = userPiEditModel.Id;

            UserPi editedUserPi = new UserPi(entryId, user, newPi.Id);

            UserPiManager uPM = new UserPiManager();
            UserPi savedUserPi = uPM.EditUserPi(editedUserPi);

            if (savedUserPi != null)
            {
                return Json(new { success = true });
            }
            else
            {
                userPiEditModel.CurrentPi = newPi;
                return PartialView("_EditPartial", userPiEditModel);
            }

        }


        public ActionResult Create()
        {
            return PartialView("_CreatePartial", new UserPiCreateModel());
        }

        [HttpPost]
        public ActionResult Create(UserPiCreateModel model)
        {
            if (doesMappingExistAndValidation(model))
            {
                string userName = model.SelectedUserName;
                string piName = model.SelectedPiName;

                SubjectManager subjectManager = new SubjectManager();
                User user = subjectManager.GetUserByName(userName);
                User pi = subjectManager.GetUserByName(piName);

                UserPiManager userPiManager = new UserPiManager();
                userPiManager.AddUserPi(user.Id, pi.Id);

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public void Delete(long id)
        {
            UserPiManager userPiManager = new UserPiManager();
            userPiManager.DeleteUserPi(id);
        }

        /// <summary>
        /// checks if a UserPi mapping already exist and if the given usernames are valid
        /// </summary>
        /// <param name="model">current model from view</param>
        /// <returns>validation result</returns>
        private bool doesMappingExistAndValidation(UserPiCreateModel model)
        {
            string userName = model.SelectedUserName;
            string piName = model.SelectedPiName;

            if (userName != null && piName != null && userName.Length > 0 && piName.Length > 0)
            {
                SubjectManager sum = new SubjectManager();
                User User = sum.GetUserByName(userName);
                User Pi = sum.GetUserByName(piName);

                if (User != null && Pi != null)
                {
                    UserPiManager upm = new UserPiManager();
                    UserPi upi = upm.GetUserPi(User.Id, Pi.Id);

                    if (upi == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
