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
    public class UnitsController : Controller
    {
        private PropertyRentalManagementEntities db = new PropertyRentalManagementEntities();

        // GET: Units
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.BNumberSortParm = String.IsNullOrEmpty(sortOrder) ? "BNumber_asc" : "";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Status_asc" : "";
            ViewBag.SizeSortParm = String.IsNullOrEmpty(sortOrder) ? "Size_asc" : "";
            ViewBag.RentSortParm = String.IsNullOrEmpty(sortOrder) ? "Rent_asc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var units = db.Units.Include(u => u.Building);


            if (!String.IsNullOrEmpty(searchString))
            {
                units = units.Where(u => u.Building.BuildingNumber.ToString().Contains(searchString)
                                       || u.Size.ToString().Contains(searchString) || u.Status.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "BNumber_asc":
                    units = units.OrderBy(b => b.Building.BuildingNumber);
                    break;
                case "Status_asc":
                    units = units.OrderBy(b => b.Status);
                    break;
                case "Size_asc":
                    units = units.OrderBy(b => b.Size);
                    break;
                case "Rent_asc":
                    units = units.OrderBy(b => b.Rents);
                    break;
                default:
                    units = units.OrderBy(b => b.UnitId);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(units.ToPagedList(pageNumber, pageSize));

        }

        // GET: Units/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // GET: Units/Create
        public ActionResult Create()
        {
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingId");
            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitId,UnitNumber,BuildingId,Rents,Size,Status")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                db.Units.Add(unit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingId", unit.BuildingId);
            return View(unit);
        }

        // GET: Units/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingId", unit.BuildingId);
            return View(unit);
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitId,UnitNumber,BuildingId,Rents,Size,Status")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuildingId = new SelectList(db.Buildings, "BuildingId", "BuildingId", unit.BuildingId);
            return View(unit);
        }

        // GET: Units/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit unit = db.Units.Find(id);
            db.Units.Remove(unit);
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
    }
}
