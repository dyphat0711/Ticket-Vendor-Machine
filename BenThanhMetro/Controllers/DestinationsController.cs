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
    public class DestinationsController : Controller
    {
        // Gọi database context thông qua Entity Framework
        private MetroTicketDBEntities db = new MetroTicketDBEntities();

        // GET: Destinations (Hiển thị danh sách các ga)
        public ActionResult Index()
        {
            // Kiểm tra nếu chưa đăng nhập hoặc không phải Admin thì đuổi về trang Login
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(db.Destinations.ToList());
        }

        // GET: Destinations/Details/5 (Xem chi tiết 1 ga)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // GET: Destinations/Create (Hiển thị Form thêm ga mới)
        public ActionResult Create()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        // POST: Destinations/Create (Xử lý khi bấm nút Lưu ga mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DestinationID,Name,FareAmount")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                db.Destinations.Add(destination);
                db.SaveChanges(); // Lưu vào SQL
                return RedirectToAction("Index"); // Quay về trang danh sách
            }

            return View(destination);
        }

        // GET: Destinations/Edit/5 (Hiển thị Form sửa ga)
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
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Edit/5 (Xử lý khi bấm nút Cập nhật ga)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DestinationID,Name,FareAmount")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(destination).State = EntityState.Modified;
                db.SaveChanges(); // Cập nhật vào SQL
                return RedirectToAction("Index");
            }
            return View(destination);
        }

        // GET: Destinations/Delete/5 (Hiển thị xác nhận xóa)
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
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Delete/5 (Thực thi lệnh xóa khỏi SQL)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Destination destination = db.Destinations.Find(id);
            db.Destinations.Remove(destination);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Giải phóng kết nối Database
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