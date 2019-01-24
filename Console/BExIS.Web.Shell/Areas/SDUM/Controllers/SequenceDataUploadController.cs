using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using BExIS.Dlm.Services.Data;
using Npgsql;
using System.Configuration;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using BExIS.Modules.Asm.UI.Models;
using BExIS.Modules.Rpm.UI.Models;
using System.Linq;
using System.Xml;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using BExIS.Modules.Rpm.UI.Controllers;
using BExIS.IO.Transform.Output;
using BExIS.Utils.Models;
using System.Data;

namespace BExIS.Modules.Sdum.UI.Controllers
{
    public class SequenceDataUploadController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        
    }
    
}