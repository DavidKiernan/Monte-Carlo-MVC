using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MonteCarloSim.DAL;
using MonteCarloSim.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
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
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder; // provides view with current sort order, keeps paging links in order to keep sort order same while paging
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "contract_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CurrSortParm = sortOrder == "curr_price" ? "curr_price_desc" : "curr_price";
            ViewBag.StrikeParm = sortOrder == "str_price" ? "str_price_desc" : "str_price";
            ViewBag.IVParm = sortOrder == "iv_price" ? "iv_price_desc" : "iv_price";
            ViewBag.RiskFreeParm = sortOrder == "risk" ? "risk_desc" : "risk";
            ViewBag.OptionTypeParm = sortOrder == "Call" ? "Put" : "Call";

            // keeps view with the current filter.value must be included in the paging links in order to maintain the filter settings during paging
            // It must be restored to the text box when the page is redisplayed
            ViewBag.CurrentFilter = searchString;

            var opt = from o in db.Options
                      select o;
            // Search by contract Name
            if (!String.IsNullOrEmpty(searchString))
            {
                // search by contract name that start with first letter entered
                opt = opt.Where(o => o.ContractName.StartsWith(searchString));
            }
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

            int pageSize = 3; // items per page 

            // ??  represent the null-coalescing operator. defines a default value for a nullable type
            //means return the value of page if it has a value, or return 1 if page is null
            int pageNumber = (page ?? 1);

            // The ToPagedList extension method on the IQueryable object
            // object converts the student query to a single page of options in a collection type that supports paging
            // That single page of options is then passed to the view:
            return View(opt.ToPagedList(pageNumber, pageSize));
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
                    ModelState.AddModelError("ExpiryDate", "Date Must Be Greater Than Today's Date"); // display this message  
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

                    option.ContractName = option.ContractName.ToUpper(); // convert the contract name to upper case
                    db.Options.Add(option); // add to the DB
                    db.SaveChanges(); // Save 
                    return RedirectToAction("Index"); // return to the option Index page
                }
            }
            catch (RetryLimitExceededException /* dex */)
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
            if (TryUpdateModel(optionToUpdate, "", new string[] { "ContractName" }))
            {
                try
                {
                    optionToUpdate.ContractName = optionToUpdate.ContractName.ToUpper();
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
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
            catch (RetryLimitExceededException/* dex */)
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
