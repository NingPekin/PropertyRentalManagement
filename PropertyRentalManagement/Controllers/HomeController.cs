using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    public class HomeController : Controller
    {
        private PropertyRentalManagementEntities db = new PropertyRentalManagementEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                var details = (from userlist in db.Users
                               where userlist.UserName == user.UserName && userlist.Password == user.Password
                               select new
                               {

                                   userlist.UserId,
                                   userlist.UserName
                               }).ToList();
                if (details.FirstOrDefault() != null)
                {
                    Session["UserId"] = details.FirstOrDefault().UserId;
                    Session["UserName"] = details.FirstOrDefault().UserName;
                    return RedirectToAction("List", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Invalid Credentials");
                }
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View();

        }
        public ActionResult About()
        {
            ViewBag.Message = "about";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact";

            return View();
        }
    }
}