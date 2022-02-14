﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using BExIS.Security.Services.Subjects;

namespace BExIS.App.Bootstrap.Helpers
{
    public  class BExISAuthorizeHelper
    {
        /// <summary>
        /// This function checks the HttpRequest Authorization string for Authorization types (Bearer & Basic). 
        /// If these exist, I rights are checked in relation to the features and a response message 
        /// is generated.If the authorization was successful, the response is null and the user is out. 
        /// </summary>
        /// <param name="authorisation">string from request header</param>
        /// <param name="featureId"></param>
        /// <param name="user"></param>
        /// <returns>HttpResponseMessage, if null, is athorized, otherwise a message is added</returns>
        public static HttpResponseMessage HttpRequestAuthorization(string authorisation, long featureId, out User user)
        {
            HttpResponseMessage response = null;

            using (var featurePermissionManager = new FeaturePermissionManager())
            using (var operationManager = new OperationManager())
            using (var userManager = new UserManager())
            {

                if (string.IsNullOrEmpty(authorisation))
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                    user = null;
                    return response;
                }

                // e.g. Bearer kajggsdagsda
                var authArray = authorisation.Split(' ');
                var type = authArray[0].ToLower();
                var code = authArray[1];

                if (type == "bearer")
                {
                    var token = code;
                    if (token != null)
                    {
                        // resolve the token to the corresponding user
                        var users = userManager.Users.Where(u => u.Token == token);

                        if (users == null || users.Count() != 1)
                        {
                            response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                            response.Content = new StringContent("Bearer token not exist.");
                        }

                        if (!featurePermissionManager.HasAccess(users.Single().Id, featureId))
                        {
                            response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                            response.Content = new StringContent("Token is not valid.");
                        }

                        user = users.FirstOrDefault();
                        return response;
                    }


                }

                if (type == "basic")
                {
                    var basic = code;
                    if (basic != null)
                    {
                        using (var identityUserService = new IdentityUserService())
                        {
                            user = userManager.FindByNameAsync(System.Text.Encoding.UTF8.GetString(
                            Convert.FromBase64String(basic)).Split(':')[0]).Result;

                            if (user != null)
                            {
                                var result = identityUserService.CheckPasswordAsync(user, System.Text.Encoding.UTF8.GetString(
                            Convert.FromBase64String(basic)).Split(':')[1]).Result;

                                if (!result)
                                {
                                    response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                                    response.Content = new StringContent("Username and/or Password are incorrect.");
                                }

                                if (!featurePermissionManager.HasAccess(user.Id, featureId))
                                {
                                    response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                                    response.Content = new StringContent("User is not valid.");

                                }

                            }

                            return response;
                        }
                    }
                }

                user = null;
                return response;

            }
        }


        //public static HttpResponseMessage HttpRequestAuthorization(HttpRequestMessage request, long featureId, out User user)
        //{
        //    HttpResponseMessage response = null;

        //    using (var featurePermissionManager = new FeaturePermissionManager())
        //    using (var operationManager = new OperationManager())
        //    using (var userManager = new UserManager())
        //    {

        //        if (request.Headers.Authorization == null)
        //        {
        //            response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //            user = null;
        //            return response;
        //        }

        //        if (request.Headers.Authorization.Scheme == "Bearer")
        //        {
        //            var token = request.Headers.Authorization?.ToString().Substring("Bearer ".Length).Trim();
        //            if (token != null)
        //            {
        //                // resolve the token to the corresponding user
        //                var users = userManager.Users.Where(u => u.Token == token);

        //                if (users == null || users.Count() != 1)
        //                {
        //                    response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //                    response.Content = new StringContent("Bearer token not exist.");
        //                }

        //                if (!featurePermissionManager.HasAccess(users.Single().Id, featureId))
        //                {
        //                    response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //                    response.Content = new StringContent("Token is not valid.");
        //                }

        //                user = users.FirstOrDefault();
        //                return response;
        //            }


        //        }

        //        if (request.Headers.Authorization.Scheme == "Basic")
        //        {
        //            var basic = request.Headers.Authorization?.ToString().Substring("Basic ".Length).Trim();
        //            if (basic != null)
        //            {
        //                using (var identityUserService = new IdentityUserService())
        //                {
        //                    user = userManager.FindByNameAsync(System.Text.Encoding.UTF8.GetString(
        //                    Convert.FromBase64String(basic)).Split(':')[0]).Result;

        //                    if (user != null)
        //                    {
        //                        var result = identityUserService.CheckPasswordAsync(user, System.Text.Encoding.UTF8.GetString(
        //                    Convert.FromBase64String(basic)).Split(':')[1]).Result;

        //                        if (!result)
        //                        {
        //                            response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //                            response.Content = new StringContent("Username and/or Password are incorrect.");
        //                        }

        //                        if (!featurePermissionManager.HasAccess(user.Id, featureId))
        //                        {
        //                            response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //                            response.Content = new StringContent("User is not valid.");

        //                        }

        //                    }

        //                    return response;
        //                }
        //            }
        //        }


        //    }

        //    user = null;
        //    return response;

        //}
    }


}
