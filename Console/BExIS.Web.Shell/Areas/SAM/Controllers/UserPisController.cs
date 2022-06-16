using BExIS.Security.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using BExIS.Security.Entities.Subjects;
using BExIS.Modules.Sam.UI.Models;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.Sam.UI.Controllers
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

        public ActionResult openCreatePiMappingWindow()
        {
            return View("UserPis");
        }

        [GridAction]
        public ActionResult UserPis_Select()
        {
            List<UserPisGridRowModel> users = new List<UserPisGridRowModel>();
            UserPiManager userPiManager = new UserPiManager();

            ICollection<UserPi> collection = userPiManager.GetAllUserPis();

            if (collection != null)
            {
                UserPisGridRowModel converter = new UserPisGridRowModel();
                users = userPiManager.GetAllUserPis().Select(upis => converter.Convert(upis)).ToList();
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
            string newPiName = userPiEditModel.SelectedPiName;

            var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
            long newPiId = repo.Query(u => u.Name.Equals(newPiName))
                        .Select( u => u.Id )
                        .FirstOrDefault();
            
            long entryId = userPiEditModel.Id;

            UserPiManager uPM = new UserPiManager();
            UserPi savedUserPi = uPM.EditUserPi(entryId, newPiId);

            if (savedUserPi != null)
            {
                return Json(new { success = true });
            }
            else
            {
                //userPiEditModel.CurrentPi = newPi;
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
                using (SubjectManager subjectManager = new SubjectManager())
                {
                    //SubjectManager subjectManager = new SubjectManager();
                    //User user = subjectManager.GetUserByName(userName);
                    //User pi = subjectManager.GetUserByName(piName);
                    var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
                    User user = repo.Query(u => u.Name.Equals(userName)).FirstOrDefault();
                    User pi = repo.Query(u => u.Name.Equals(piName)).FirstOrDefault();

                    UserPiManager userPiManager = new UserPiManager();
                    userPiManager.AddUserPi(user.Id, pi.Id);

                    return Json(new { success = true });
                }
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
                using (SubjectManager subjectManager = new SubjectManager())
                {
                    //SubjectManager sum = new  SubjectManager();
                    //User User = sum.GetUserByName(userName);
                    //User Pi = sum.GetUserByName(piName);
                    var repo = this.GetUnitOfWork().GetReadOnlyRepository<User>();
                    User User = repo.Query(u => u.Name.Equals(userName)).FirstOrDefault();
                    User Pi = repo.Query(u => u.Name.Equals(piName)).FirstOrDefault();

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
            }

            return false;
        }
    }
}
