﻿using BExIS.App.Bootstrap.Attributes;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using BExIS.Utils.Route;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BExIS.Web.Shell.Controllers.API
{
    public class TokenController : ApiController
    {
        [BExISApiAuthorize]
        [GetRoute("api/Token/")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                using (var userManager = new UserManager())
                {
                    var user = ControllerContext.RouteData.Values["user"] as User;
                    if (user != null)
                    {
                        if (user.Token == null)
                        {
                            await userManager.SetTokenAsync(user);
                            user = userManager.FindByIdAsync(user.Id).Result;
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, new { token = user.Token });
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}