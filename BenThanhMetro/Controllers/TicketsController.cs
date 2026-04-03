using BenThanhMetro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BenThanhMetro.Controllers
{
    public class TicketsController : Controller
    {
        private MetroTicketDBEntities db = new MetroTicketDBEntities();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Destination).Include(t => t.TicketVendorMachine);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // 2. Màn hình Chọn Ga 
        public ActionResult Destinations()
        {
            // Tự động query "SELECT * FROM Destination" và đưa vào List
            List<Destination> listDest = db.Destinations.ToList();

            // Truyền danh sách listDest ra giao diện (View) để hiển thị các nút bấm
            return View(listDest);
        }

        // 3. Màn hình Chọn Phương Thức Thanh Toán
        [HttpPost]
        public ActionResult PaymentMethod(int destId, string destName, decimal price)
        {
            // Nhận dữ liệu từ form và lưu tạm vào TempData
            TempData["DestID"] = destId;
            TempData["DestName"] = destName;
            TempData["Price"] = price;

            // Trả về file View PaymentMethod.cshtml mà bạn vừa tạo
            return View();
        }

        // 3A. Màn hình rẽ nhánh: Chọn cách dùng Thẻ Tín Dụng
        [HttpPost]
        public ActionResult CreditCardPayment()
        {
            // Giữ lại TempData để không bị mất thông tin Ga và Giá tiền khi chuyển trang
            TempData.Keep("DestID");
            TempData.Keep("DestName");
            TempData.Keep("Price");

            return View();
        }

        // 3B. Màn hình rẽ nhánh: Quét mã QR Ví điện tử
        [HttpPost]
        public ActionResult QRPayment(string walletType)
        {
            TempData.Keep("DestID");
            TempData.Keep("DestName");
            TempData.Keep("Price");

            // Truyền loại ví (VNPay, Momo...) ra View để hiển thị logo cho đúng
            ViewBag.WalletType = walletType;
            return View();
        }

        [HttpPost]
        public ActionResult Processing(string method)
        {
            // Tiếp tục giữ lại các thông tin Ga và Giá tiền
            TempData.Keep("DestID");
            TempData.Keep("DestName");
            TempData.Keep("Price");

            // Lưu lại phương thức thanh toán khách vừa chọn để truyền tiếp sang bước sau
            TempData["Method"] = method;

            return View();
        }

        // 3.5. Màn hình rẽ nhánh: Chọn loại Ví điện tử (MoMo, VNPay, ZaloPay)
        [HttpPost]
        public ActionResult SelectWallet()
        {
            // Giữ lại thông tin Ga và Giá tiền
            TempData.Keep("DestID");
            TempData.Keep("DestName");
            TempData.Keep("Price");

            return View();
        }

        // 4. Xử lý Giao dịch (Lưu DB) và Chuyển sang màn hình Thành công
        [HttpPost]
        public ActionResult ProcessPayment(string method)
        {
            int destId = Convert.ToInt32(TempData["DestID"]);
            decimal amount = Convert.ToDecimal(TempData["Price"]);
            int machineId = 1; // Giả sử khách đang thao tác ở máy "Ben Thanh Station - Gate A"

            // Sinh mã vạch ngẫu nhiên (Ví dụ: BT-A9B8C7)
            string barcode = "BT-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            DateTime validUntil = DateTime.Now.AddHours(4);

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert bảng Ticket
                        string sqlTicket = @"INSERT INTO Ticket (ValidUntil, BarcodeData, DestinationID, MachineID) 
                                             OUTPUT INSERTED.TicketID 
                                             VALUES (@ValidUntil, @Barcode, @DestID, @MachineID)";
                        System.Data.SqlClient.SqlCommand cmdTicket = new System.Data.SqlClient.SqlCommand(sqlTicket, conn, transaction);
                        cmdTicket.Parameters.AddWithValue("@ValidUntil", validUntil);
                        cmdTicket.Parameters.AddWithValue("@Barcode", barcode);
                        cmdTicket.Parameters.AddWithValue("@DestID", destId);
                        cmdTicket.Parameters.AddWithValue("@MachineID", machineId);

                        int newTicketId = (int)cmdTicket.ExecuteScalar();

                        // Insert bảng Payment
                        string sqlPayment = @"INSERT INTO Payment (TicketID, Amount, PaymentMethod, TransactionStatus) 
                                              VALUES (@TicketID, @Amount, @Method, 'Completed')";
                        System.Data.SqlClient.SqlCommand cmdPay = new System.Data.SqlClient.SqlCommand(sqlPayment, conn, transaction);
                        cmdPay.Parameters.AddWithValue("@TicketID", newTicketId);
                        cmdPay.Parameters.AddWithValue("@Amount", amount);
                        cmdPay.Parameters.AddWithValue("@Method", method);

                        cmdPay.ExecuteNonQuery();

                        // Hoàn tất lưu dữ liệu
                        transaction.Commit();

                        // Gửi dữ liệu ra màn hình Thành công
                        ViewBag.Barcode = barcode;
                        ViewBag.ValidUntil = validUntil.ToString("dd/MM/yyyy HH:mm");
                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ViewBag.Error = "Giao dịch thất bại: " + ex.Message;
                        return View("Error"); // Bạn có thể tạo thêm View Error nếu muốn
                    }
                }
            }
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