using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagement.Models;
using PagedList;

namespace PropertyRentalManagement.Controllers
{
    public class RentalsController : Controller
    {
        private PropertyRentalManagementEntities db = new PropertyRentalManagementEntities();
    

        // GET: Rentals
        public ActionResult Index()
        {
            var rentals = db.Rentals.Include(r => r.Unit).Include(r => r.User);
            return View(rentals.ToList());
        }

        // GET: Rentals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Rentals/Create
        public ActionResult Create()
        {
                ViewBag.UnitId = new SelectList(db.Units.Where(u=>u.Status!=2), "UnitId", "UnitId");
                ViewBag.UserId = new SelectList(db.Users.Where(u=>u.Type==2), "UserId", "UserName");
                ViewBag.Tenant_Name = new SelectList(from x in db.Users where x.Type == 2 select x.UserId).ToList();
                return View();
            
            }

        // POST: Rentals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RentalId,UnitId,UserId")] Rental rental)
        {

        
            if (ModelState.IsValid)
            {
                var validUnit = (from x in db.Units where x.UnitId==rental.UnitId&& x.Status != 2 select x).FirstOrDefault();
                //var dulpliUser = from x in db.Rentals where x.UnitId==rental.UnitId&&x.User.UserName == rental.User.UserName select x;
                //var dulpliUser = (from x in db.Rentals where x.UnitId == rental.UnitId && x.User.UserName.Equals(rental.User.UserName) select x).Count();

                
                if (validUnit != null)
                {
                    //if (dulpliUser ==0)
                    //{
                        db.Rentals.Add(rental);
                        //update to occupied
                        (from u in db.Units
                         where u.UnitId == rental.UnitId
                         select u).ToList().ForEach(x => x.Status = 2);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    //}

                    //else
                    //{
                    //    ModelState.AddModelError("", "This Tenant is in the list!");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "This unit is RENTED!");
                }
                }
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "UnitId", rental.UnitId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", rental.UserId);
            ViewBag.Tenant_Name = new SelectList(from x in db.Users where x.Type == 2 select x.UserId).ToList();
            return View(rental);
        }

        // GET: Rentals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status != 2), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            ViewBag.Tenant_Name = new SelectList(from x in db.Users where x.Type == 2 select x.UserName).ToList();
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RentalId,UnitId,UserId")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                var valid = (from x in db.Units where x.UnitId == rental.UnitId && x.Status != 2 select x).FirstOrDefault();
                if (valid != null)
                {
                    db.Entry(rental).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "This unit is RENTED!");
                }

            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status != 2), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);
            db.Rentals.Remove(rental);
            (from u in db.Units
             where u.UnitId == rental.UnitId
             select u).ToList().ForEach(x => x.Status = 0);
            db.SaveChanges();
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


        public ActionResult SearchView(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UnitSortParm = String.IsNullOrEmpty(sortOrder) ? "Unit_asc" : "";
            ViewBag.UserSortParm = String.IsNullOrEmpty(sortOrder) ? "User_asc" : "";
       
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var rentals = db.Rentals.Include(r => r.Unit).Include(r => r.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                rentals = rentals.Where(u => u.Unit.UnitNumber.ToString().Contains(searchString)
                                       || u.User.UserName.Contains(searchString) );
            }
            switch (sortOrder)
            {
                case "Unit_asc":
                    rentals = rentals.OrderBy(b => b.Unit.UnitNumber);
                    break;
                case "User_asc":
                    rentals = rentals.OrderBy(b => b.User.UserName);
                    break;
                default:
                    rentals = rentals.OrderBy(b => b.RentalId);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(rentals.ToPagedList(pageNumber, pageSize));

        }


    }
}
