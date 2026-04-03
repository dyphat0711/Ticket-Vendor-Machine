using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BenThanhMetro.Models;

namespace BenThanhMetro.Controllers
{
    public class TicketVendorMachinesController : Controller
    {
        private MetroTicketDBEntities db = new MetroTicketDBEntities();

        // GET: TicketVendorMachines
        public ActionResult Index()
        {
            // Kiểm tra nếu chưa đăng nhập hoặc không phải Admin thì đuổi về trang Login
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            return View(db.TicketVendorMachines.ToList());
        }

        // GET: TicketVendorMachines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketVendorMachine ticketVendorMachine = db.TicketVendorMachines.Find(id);
            if (ticketVendorMachine == null)
            {
                return HttpNotFound();
            }
            return View(ticketVendorMachine);
        }

        // GET: TicketVendorMachines/Create
        public ActionResult Create()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        // POST: TicketVendorMachines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MachineID,Location,Status")] TicketVendorMachine ticketVendorMachine)
        {
            if (ModelState.IsValid)
            {
                db.TicketVendorMachines.Add(ticketVendorMachine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketVendorMachine);
        }

        // GET: TicketVendorMachines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketVendorMachine ticketVendorMachine = db.TicketVendorMachines.Find(id);
            if (ticketVendorMachine == null)
            {
                return HttpNotFound();
            }
            return View(ticketVendorMachine);
        }

        // POST: TicketVendorMachines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MachineID,Location,Status")] TicketVendorMachine ticketVendorMachine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketVendorMachine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketVendorMachine);
        }

        // GET: TicketVendorMachines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketVendorMachine ticketVendorMachine = db.TicketVendorMachines.Find(id);
            if (ticketVendorMachine == null)
            {
                return HttpNotFound();
            }
            return View(ticketVendorMachine);
        }

        // POST: TicketVendorMachines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketVendorMachine ticketVendorMachine = db.TicketVendorMachines.Find(id);
            db.TicketVendorMachines.Remove(ticketVendorMachine);
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