using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    public class AppointmentsController : Controller
    {
        private PropertyRentalManagementEntities db = new PropertyRentalManagementEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            var appointments = db.Appointments.Include(a => a.Unit).Include(a => a.User);
            return View(appointments.ToList());
        }

        public ActionResult TenantIndex()
        {
            //only show tenant appointment
            int id= (int)Session["UserId"];
            var appointments = db.Appointments.Include(a => a.Unit).Include(a => a.User).Where(x=>x.UserId.Equals (id));
            return View(appointments.ToList());
        }
        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/TenantDetails/5
        public ActionResult TenantDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }


        // GET: Appointments/Create
        public ActionResult Create()
        {
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentId,Date,Time,UnitId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                //update to pending
                (from u in db.Units
                 where u.UnitId == appointment.UnitId
                 select u).ToList().ForEach(x => x.Status = 1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            return View(appointment);
        }


        // GET: Appointments/TenantCreate
        public ActionResult TenantCreate()
        {
            int id = (int)Session["UserId"];
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.UserId.Equals(id)), "UserId", "UserName");

           
            return View();
        }

        // POST: Appointments/TenantCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TenantCreate([Bind(Include = "AppointmentId,Date,Time,UnitId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                //update to pending
                (from u in db.Units
                 where u.UnitId == appointment.UnitId
                 select u).ToList().ForEach(x => x.Status = 1);
                db.SaveChanges();
                return RedirectToAction("TenantIndex");
            }

            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            int id = (int)Session["UserId"];
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.UserId.Equals(id)), "UserId", "UserName");
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,Date,Time,UnitId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Type == 2), "UserId", "UserName");
            return View(appointment);
        }


        // GET: Appointments/Edit/5
        public ActionResult TenantEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            int userid = (int)Session["UserId"];
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.UserId.Equals(userid)), "UserId", "UserName");
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TenantEdit([Bind(Include = "AppointmentId,Date,Time,UnitId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitId = new SelectList(db.Units.Where(u => u.Status == 0), "UnitId", "UnitId");
            int id = (int)Session["UserId"];
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.UserId.Equals(id)), "UserId", "UserName");
            return View(appointment);
        }



        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            (from u in db.Units
             where u.UnitId == appointment.UnitId
             select u).ToList().ForEach(x => x.Status = 0);
            db.SaveChanges();
            return RedirectToAction("Index");
            
            }

        // GET: Appointments/TenantDelete/5
        public ActionResult TenantDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("TenantDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult TenantDeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            (from u in db.Units
             where u.UnitId == appointment.UnitId
             select u).ToList().ForEach(x => x.Status = 0);
            db.SaveChanges();
            return RedirectToAction("TenantIndex");

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
