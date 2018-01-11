using System.Web.Mvc;
using Vaiona.Web.Mvc.Data;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Web.Shell.Controllers
{
    public class HomeController : Controller
    {
        [DoesNotNeedDataAccess]
        public ActionResult Index()
        {
            if (!this.IsAccessibale("DDM", "Home", "Index")) return View();

            var result = this.Render("DDM", "Home", "Index");
            return Content(result.ToHtmlString(), "text/html");
        }

        [DoesNotNeedDataAccess]
        public ActionResult SessionTimeout()
        {
            return View();
        }
        
        [DoesNotNeedDataAccess]
        public ActionResult RedirectToWiki()
        {
            return Redirect("https://aquadiva-trac1.inf-bb.uni-jena.de/wiki/doku.php");
        }

        [DoesNotNeedDataAccess]
        public ActionResult RedirectToBugtracker()
        {
            return Redirect("https://aquadiva-trac1.inf-bb.uni-jena.de/mantis/bug_report_page.php");
        }
    }
}
