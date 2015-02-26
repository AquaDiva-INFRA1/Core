﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using BExIS.Web.Shell.Areas.Auth.Models;
using Telerik.Web.Mvc;

namespace BExIS.Web.Shell.Areas.Auth.Controllers
{
    public class GroupsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // --------------------------------------------------
        // GROUPS
        // --------------------------------------------------

        #region Groups

        public ActionResult Groups()
        {
            return View();
        }

        [GridAction]
        public ActionResult Groups_Select()
        {
            SubjectManager subjectManager = new SubjectManager();
            List<GroupGridRowModel> groups = subjectManager.GetAllGroups().Select(g => GroupGridRowModel.Convert(g)).ToList();

            return View(new GridModel<GroupGridRowModel> { Data = groups });
        }

        #endregion


        [HttpPost]
        public void Delete(long id)
        {
            SubjectManager subjectManager = new SubjectManager();
            subjectManager.DeleteGroupById(id);
        }

        public ActionResult Edit(long id)
        {
            SubjectManager subjectManager = new SubjectManager();

            Group group = subjectManager.GetGroupById(id);

            return PartialView("_EditPartial", GroupEditModel.Convert(group));
        }

        [HttpPost]
        public ActionResult Edit(GroupEditModel model, long[] selectedUsers)
        {
            if (ModelState.IsValid)
            {
                SubjectManager subjectManager = new SubjectManager();

                Group group = subjectManager.GetGroupById(model.GroupId);

                group.Name = model.GroupName;
                group.Description = model.Description;

                long[] users = group.Users.Select(g => g.Id).ToArray();

                if (users != null)
                {
                    foreach (long userId in users)
                    {
                        subjectManager.RemoveUserFromGroup(userId, group.Id);
                    }
                }

                if (selectedUsers != null)
                {
                    foreach (long userId in selectedUsers)
                    {
                        subjectManager.AddUserToGroup(userId, group.Id);
                    }
                }

                subjectManager.UpdateGroup(group);

                return Json(new { success = true });
            }
            else
            {
                ViewData["SelectedUsers"] = selectedUsers;

                return PartialView("_EditPartial", model);
            }
        }

        [GridAction]
        public ActionResult Membership_Select(long id, long[] selectedUsers)
        {
            SubjectManager subjectManager = new SubjectManager();

            List<GroupMembershipGridRowModel> users = new List<GroupMembershipGridRowModel>();

            if (selectedUsers != null)
            {
                users = subjectManager.GetAllUsers().Select(u => GroupMembershipGridRowModel.Convert(u, selectedUsers.Contains(u.Id))).ToList();
            }
            else
            {
                Group group = subjectManager.GetGroupById(id);

                users = subjectManager.GetAllUsers().Select(u => GroupMembershipGridRowModel.Convert(u, u.Groups.Any(g => g.Id == id))).ToList();
            }

            return View(new GridModel<GroupMembershipGridRowModel> { Data = users });
        }

        #region Grid View

        // C
        public ActionResult Create()
        {
            return PartialView("_CreatePartial");
        }

        [HttpPost]
        public ActionResult Create(GroupCreateModel model)
        {
            if (ModelState.IsValid)
            {
                SubjectManager subjectManager = new SubjectManager();
                subjectManager.CreateGroup(model.GroupName, model.Description);

                return Json(new { success = true });
            }

            return PartialView("_CreatePartial", model);
        }

        #endregion


        #region Validation

        public JsonResult ValidateGroupName(string groupName, long groupId = 0)
        {
            SubjectManager subjectManager = new SubjectManager();

            Group group = subjectManager.GetGroupByName(groupName);

            if (group == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (group.Id == groupId)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string error = String.Format(CultureInfo.InvariantCulture, "The group name already exists.", groupName);

                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion

    }
}
