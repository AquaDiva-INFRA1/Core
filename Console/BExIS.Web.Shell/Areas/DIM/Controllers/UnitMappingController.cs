using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BExIS.Modules.Dim.UI.Controllers
{
    public class UnitMappingController : Controller
    {
        // GET: MappingUnit
        public ActionResult Index()
        {
            using (UnitManager unit_manag = new UnitManager())
            {
                List<Dlm.Entities.DataStructure.Unit> repo = unit_manag.Repo.Get().OrderBy(x => x.Name).ToList();
                List<ConversionMethod> conversionmethods = new List<ConversionMethod>();
                repo.ForEach(r =>
                {
                    conversionmethods.AddRange(r.ConversionsIamTheSource);
                });
                ViewData["conversions"] = conversionmethods.OrderBy(x => x.Source.Name).ToList();
                ViewData["repo"] = repo;
                return View(conversionmethods.OrderBy(x => x.Source.Name).ToList());
            }
        }

        // GET: MappingUnit/Create
        public ActionResult Create()
        {
            using (UnitManager unit_manag = new UnitManager())
            {
                ViewData["units"] = unit_manag.Repo.Get().OrderBy(x => x.Name).ToList();
                return View();
            }
        }

        // POST: MappingUnit/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string Description = Convert.ToString(collection["Description"]);
                string Formula = Convert.ToString(collection["Formula"]);
                Int64 unitsource = Int64.Parse(collection["Source"]);
                Int64 unitTarget = Int64.Parse(collection["Target"]);
                using (UnitManager unit_manag = new UnitManager())
                {
                    if (unitsource == unitTarget)
                    {
                        ConversionMethod cm = new ConversionMethod();
                        cm.Formula = Formula;
                        cm.Description = Description;
                        cm.Source = unit_manag.Repo.Get(unitsource);
                        cm.Target = unit_manag.Repo.Get(unitTarget);
                        ViewData["units"] = unit_manag.Repo.Get().OrderBy(x => x.Name).ToList();
                        ViewData["errors"] ="Source and Target should be different";
                        return View(cm);
                    }
                    Dlm.Entities.DataStructure.Unit unit = unit_manag.Repo.Get(unitsource);
                    ConversionMethod conversionmethod = unit_manag.CreateConversionMethod(Formula, Description, unit, unit_manag.Repo.Get(unitTarget));
                    unit.ConversionsIamTheSource.Add(conversionmethod);
                    unit_manag.Update(unit);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        // GET: MappingUnit/Edit/5
        public ActionResult Edit(long id)
        {
            using (UnitManager unit_manag = new UnitManager())
            {
                ViewData["units"] = unit_manag.Repo.Get().OrderBy(x => x.Name).ToList();
                return View(unit_manag.Repo.Get(id).ConversionsIamTheSource.FirstOrDefault(x => x.Source.Id == id));
            }
        }

        // POST: MappingUnit/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                string Description = Convert.ToString(collection["Description"]);
                string Formula = Convert.ToString(collection["Formula"]);
                Int64 unitsource = Int64.Parse(collection["Source"]);
                Int64 unitTarget = Int64.Parse(collection["Target"]);
                using (UnitManager unit_manag = new UnitManager())
                {
                    if (unitsource == unitTarget)
                    {
                        ConversionMethod cm = new ConversionMethod();
                        cm.Formula = Formula;
                        cm.Description = Description;
                        cm.Source = unit_manag.Repo.Get(unitsource);
                        cm.Target = unit_manag.Repo.Get(unitTarget);
                        ViewData["units"] = unit_manag.Repo.Get().OrderBy(x => x.Name).ToList();
                        ViewData["errors"] = "Source and Target should be different";
                        return View(cm);
                    }
                    Dlm.Entities.DataStructure.Unit unit = unit_manag.Repo.Get(unitsource);
                    ConversionMethod conversionmethod = unit.ConversionsIamTheSource.FirstOrDefault(x => x.Source.Id == id);
                    if (conversionmethod != null)
                    {
                        conversionmethod.Source = unit;
                        conversionmethod.Target = unit_manag.Repo.Get(unitTarget);
                        conversionmethod.Formula = Formula;
                        conversionmethod.Description = Description;
                        conversionmethod = unit_manag.UpdateConversionMethod(conversionmethod);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        conversionmethod = unit_manag.CreateConversionMethod(Formula, "", unit, unit_manag.Repo.Get(unitTarget));
                        unit.ConversionsIamTheSource.Add(conversionmethod);
                        unit_manag.Update(unit);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        // POST: MappingUnit/Delete/5
        [HttpPost]
        public ActionResult Delete(long sourceid , long targetid )
        {
            try
            {
                using (UnitManager unit_manag = new UnitManager())
                {
                    Dlm.Entities.DataStructure.Unit unit = unit_manag.Repo.Get(sourceid);
                    ConversionMethod conversionmethod = unit.ConversionsIamTheSource.FirstOrDefault(x => (x.Source.Id == sourceid)&&(x.Target.Id == targetid));
                    unit_manag.DeleteConversionMethod(conversionmethod);
                    unit.ConversionsIamTheSource.Remove(conversionmethod);
                    unit_manag.Update(unit);
                    return RedirectToAction("Index");
                }
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
