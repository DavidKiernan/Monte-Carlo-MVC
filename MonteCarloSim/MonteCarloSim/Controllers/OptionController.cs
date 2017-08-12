using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonteCarloSim.DAL;
using MonteCarloSim.Models;

namespace MonteCarloSim.Controllers
{
    public class OptionController : Controller
    {
        // instantiates a DB context Object
        private OptionContext db = new OptionContext();

        // GET: Option
        public ActionResult Index()
        {
            // gets list of options from the Options entity
            // by reading the Options property of DB context
            return View(db.Options.ToList());
        }

        // GET: Option/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Option option = db.Options.Find(id);
            if (option == null)
            {
                return HttpNotFound();
            }
            return View(option);
        }

        // GET: Option/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Option/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContractName,ExpriryDate,CurrentPrice,StrikePrice,ImpliedVolatility,RiskFreeRate,OptionType")] Option option)
        {
            // ID removed from bind as its PK and auto done by DB
            try
            {
                if (ModelState.IsValid) // server side validation
                {
                    option.OptPrices = new List<OptionPrice>();
                    OptionPrice optionPrices = new OptionPrice();

                    //  Call
                    if (option.OptionType == OptionType.Call)
                    {
                        // loop through difference in days
                        for (double day = 1; day < ((option.ExpriryDate - DateTime.Now).Days + 1); day++)
                        {
                            /* This is working correctly but it it over riding the pervious date & Price
                             * Eg. debugging will show value of 1.0, 0.99, 1.1 but will override that only 1.1 is the value added
                             */
                            optionPrices.Price = option.callOption(option.CurrentPrice, option.StrikePrice, option.RiskFreeRate, option.ImpliedVolatility, day);
                            optionPrices.Day = DateTime.Now.AddDays(day);
                            option.OptPrices.Add(optionPrices);

                        }
                        db.Options.Add(option);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(option);
        }

        // GET: Option/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Option option = db.Options.Find(id);
            if (option == null)
            {
                return HttpNotFound();
            }
            return View(option);
        }

        // POST: Option/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var optionToUpdate = db.Options.Find(id);
            if (TryUpdateModel(optionToUpdate, "", new string[] { "ContractName", "ExpriryDate", "CurrentPrice", "StrikePrice", "ImpliedVolatility", "RiskFreeRate", "OptionType" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(optionToUpdate);
        }

        // GET: Option/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Option option = db.Options.Find(id);
            if (option == null)
            {
                return HttpNotFound();
            }
            return View(option);
        }

        // POST: Option/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Option option = db.Options.Find(id);
                db.Options.Remove(option);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
