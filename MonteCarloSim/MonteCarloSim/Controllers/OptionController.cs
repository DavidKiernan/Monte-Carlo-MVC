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
        /*
         * The Index recives the sort order parameter from the query string in the URL
         * The default is contract name ascending. When user selects column heading the appropriate sortOrder is provided in the query string
         * The Viewbag variables used so the view can configure the heading column with the query string.
         * 
       */
        // GET: Option
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "contract_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CurrSortParm = sortOrder == "curr_price" ? "curr_price_desc" : "curr_price";
            ViewBag.StrikeParm = sortOrder == "str_price" ? "str_price_desc" : "str_price";
            ViewBag.IVParm = sortOrder == "iv_price" ? "iv_price_desc" : "iv_price";
            ViewBag.RiskFreeParm = sortOrder == "risk" ? "risk_desc" : "risk";
            ViewBag.OptionTypeParm = sortOrder == "Call" ? "Put" : "Call";
            
            var opt = from o in db.Options
                      select o;

            switch (sortOrder)
            {
                case "name_desc":
                    // Linq statement 
                    opt = opt.OrderByDescending(o => o.ContractName);
                    break;
                case "Date":
                    opt = opt.OrderBy(o => o.ExpiryDate);
                    break;
                case "date_desc":
                    opt = opt.OrderByDescending(o => o.ExpiryDate);
                    break;
                case "curr_price":
                    opt = opt.OrderBy(o => o.CurrentPrice);
                    break;
                case "curr_price_desc":
                    opt = opt.OrderByDescending(o => o.CurrentPrice);
                    break;
                case "str_price":
                    opt = opt.OrderBy(o => o.StrikePrice);
                    break;
                case "str_price_desc":
                    opt = opt.OrderByDescending(o => o.StrikePrice);
                    break;
                case "iv_price":
                    opt = opt.OrderBy(o => o.ImpliedVolatility);
                    break;
                case "iv_price_desc":
                    opt = opt.OrderByDescending(o => o.ImpliedVolatility);
                    break;
                case "risk":
                    opt = opt.OrderBy(o => o.RiskFreeRate);
                    break;
                case "risk_desc":
                    opt = opt.OrderByDescending(o => o.RiskFreeRate);
                    break;
                case "Call":
                    opt = opt.OrderBy(o => o.OptionType);
                    break;
                case "Put":
                    opt = opt.OrderByDescending(o => o.OptionType);
                    break;
                default:
                    opt = opt.OrderBy(o => o.ContractName);
                    break;
            }
            // Query not converted until executed  the opt.ToList
            //Code results in single query, not executed until the return view  
            // gets list of options based on the opt.ToList
            return View(opt.ToList());
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
        public ActionResult Create([Bind(Include = "ContractName,ExpiryDate,CurrentPrice,StrikePrice,ImpliedVolatility,RiskFreeRate,OptionType")]Option option)
        {
            // ID removed from bind as its PK and auto done by DB
            try
            {
                if (option.ExpiryDate <= DateTime.Now) // Date selected has to be greater than Todays Date
                {
                    ModelState.AddModelError("ExpiryDate", "Date Must Be Greater Than Today's Date"); // display this meeage  
                }

                if (ModelState.IsValid) // server side validation
                {

                    //  Call
                    if (option.OptionType == OptionType.Call)
                    {
                        option.CreateCall(option.CurrentPrice, option.StrikePrice, option.RiskFreeRate, option.ImpliedVolatility, option.ExpiryDate);
                    }
                    else // Put
                    {
                        option.CreatePut(option.CurrentPrice, option.StrikePrice, option.RiskFreeRate, option.ImpliedVolatility, option.ExpiryDate);
                    }

                    db.Options.Add(option);
                    db.SaveChanges();
                    return RedirectToAction("Index");
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
            if (TryUpdateModel(optionToUpdate, "", new string[] { "ContractName", "ExpiryDate", "CurrentPrice", "StrikePrice", "ImpliedVolatility", "RiskFreeRate", "OptionType" }))
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
