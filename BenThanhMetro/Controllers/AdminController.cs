using System.Linq;
using System.Web.Mvc;
using BenThanhMetro.Models; // Đảm bảo đúng namespace Models của bạn

namespace BenThanhMetro.Controllers
{
    public class AdminController : Controller
    {
        private MetroTicketDBEntities db = new MetroTicketDBEntities();

        // GET: Admin/Login (Hiển thị form đăng nhập)
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login (Xử lý khi bấm nút Đăng nhập)
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kiểm tra xem có tài khoản nào khớp trong Database không
            var user = db.tblUsers.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Nếu đúng: Tạo Session lưu quyền Admin
                Session["Role"] = user.Role;
                Session["Username"] = user.Username;

                // Chuyển hướng thẳng vào trang Quản lý Tuyến đường
                return RedirectToAction("Index", "Destinations");
            }
            else
            {
                // Nếu sai: Báo lỗi
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }
        }

        // GET: Admin/Logout (Xử lý đăng xuất)
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa toàn bộ thông tin đăng nhập
            return RedirectToAction("Index", "Tickets"); // Đẩy về lại màn hình Kiosk
        }
    }
}