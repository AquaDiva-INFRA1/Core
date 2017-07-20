﻿using BExIS.Dim.Entities.Publication;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BExIS.Dim.Helpers.GFBIO
{
    public class GFBIOWebserviceManager : BasicWebService
    {
        public Broker Broker { get; set; }

        private string pathToApi = @"api/jsonws/GFBioProject-portlet.";
        private string addtionalPath = @"request-json";


        public GFBIOWebserviceManager(Broker broker)
        {
            Broker = broker;
        }

        #region Get

        //getUser
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> GetUserByEmail(string email)
        {
            string functionName = "get-user-by-email-address";
            string entityName = "userextension";
            //string addtionalPathX = @"json";

            string url = @"" + Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/";
            string json = "{\"emailaddress\":\"" + email + "\"}";
            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        //get project
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetProjectById(long id)
        {
            string functionName = "get-project-by-id";
            string entityName = "project";
            string parameterName = "projectid";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/";
            string json = "{\"" + parameterName + "\":" + id + "}";
            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        //Get projects by username
        public async Task<string> GetProjectsByUser(long id)
        {
            string functionName = "get-projects-by-user";
            string entityName = "project";
            string parameterName = "userid";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/"; ;
            string json = "{\"" + parameterName + "\":" + id + "}";
            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        //get researchObject
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetResearchObjectById(long id)
        {
            string functionName = "get-research-object-by-id";
            string entityName = "researchobject";
            string parameterName = "researchobjectid";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/"; ;
            string json = "[{\"" + parameterName + "\":" + id + "}]";
            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        public async Task<string> GetStatusByResearchObjectById(long id)
        {
            string functionName = "get-status-by-research-object-id-and-version";
            string entityName = "submission";
            string parameterName = "researchobjectid";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/"; ;
            string json = "{\"" + parameterName + "\":" + id + "}";
            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        #endregion

        #region Set

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<string> CreateProject(long userid, string name, string description)
        {
            string functionName = "create-project";
            string entityName = "project";
            addtionalPath = @"request-json";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/";

            string json = "{\"userid\":" + userid + ",\"name\":\"" + name + "\",\"description\":\"" + description + "\"}";

            string encodedParameters = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, encodedParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="submitterid"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="researchobjecttype"></param>
        /// <param name="authornames"></param>
        /// <returns></returns>
        public async Task<string> CreateResearchObject(long userid, long projectid, string name, string description, string researchobjecttype, string extendedData, string[] authornames)
        {
            string functionName = "create-research-object";
            string entityName = "researchobject";

            string url = Broker.Server + "/" + pathToApi + entityName + "/" + functionName + "/" + addtionalPath + "/";

            string json = "[{\"userid\":" + userid + "," +
                          "\"name\":\"" + name + "\"," +
                          "\"projectid\":" + projectid + "," +
                          "\"description\":\"" + description + "\"," +
                          "\"extendeddata\":{" + extendedData + "}," +
                          "\"researchobjecttype\":\"" + researchobjecttype + "\"}]";
            //"," +
            //"\"authornames\":[" + string.Join(",", authornames) + "]}";

            //json = "[{ \"name\":\"test\",\"description\":\"test description\",\"submitterid\":16297,\"researchobjecttype\":\"x\"}]";

            string body = WebServiceHelper.Encode(json);

            return await BasicWebService.Call(url, Broker.UserName, Broker.Password, "", body);
        }

        #endregion

    }

    public class GFBIOResearchObjectJSON
    {
        public long submitterid { get; set; }
        public long projectid { get; set; }

        // length 200
        public string name { get; set; }
        // length 15000
        public string description { get; set; }
        public string researchobjecttype { get; set; }
        // length 1500
        public List<string> authornames { get; set; }
        // length 1500
        public string extendeddata { get; set; }
        public string metadatalabel { get; set; }

        public GFBIOResearchObjectJSON()
        {
            submitterid = 0;
            projectid = 0;
            name = "";
            description = "";
            researchobjecttype = "";
            authornames = new List<string>();
            extendeddata = "";
            metadatalabel = "";
        }
    }

    public class GFBIOUser
    {
        public long userid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { get; set; }
        public string emailaddress { get; set; }
        public string fullname { get; set; }
        public string screenname { get; set; }
    }

    public class GFBIOResearchObjectResult
    {
        public long researchobjectid { get; set; }
        public long researchobjectversionid { get; set; }
    }

    public class GFBIOResearchObjectStatus
    {
        public long researchobjectid { get; set; }
        public string status { get; set; }
    }

    public class GFBIOProject
    {
        public long projectid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class GFBIOException
    {
        public string exception { get; set; }
        public string message { get; set; }
    }
}
