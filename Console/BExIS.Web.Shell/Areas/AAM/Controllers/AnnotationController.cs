using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BExIS.Modules.Bam.UI.Controllers
{
    public class AnnotationController : Controller
    {
        // GET: Annotation
        public ActionResult Index()
        {
            AnnotationManager am = new AnnotationManager();
            List<Annotation> annotationList = am.GetAnnotations().ToList();
            return View(annotationList);
        }
    }
}