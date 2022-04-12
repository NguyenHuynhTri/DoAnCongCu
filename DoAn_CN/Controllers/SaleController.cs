using DemoVNPay.Others;
using DoAn_CN.Models;
using MoMo;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_CN.Controllers
{
    public class SaleController : Controller
    {
        dbDoAnCNDataContext data = new dbDoAnCNDataContext();
        private List<SanPham> ProductNew(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.NgayThem).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
        public ActionResult AllProduct(int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var Allproduct = from ALL in data.SanPhams select ALL;
            return View(Allproduct.ToPagedList(pagenum, pagesize));
        }
        public ActionResult SPTheoLoai(int id, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var theoloai = from sp in data.SanPhams where sp.idLoai == id select sp;
            return View(theoloai.ToPagedList(pagenum, pagesize));
        }
        public ActionResult chitietSP(int id)
        {
            var CTSP = from CT in data.SanPhams where CT.idSP == id select CT;
            return View(CTSP.Single());
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {

            var all = data.SanPhams.Where(x => x.TenSP.Contains(keyword));

            return View(all);
        }
        /*=======================================================
        ========================Giỏ hàng=========================
        =======================================================*/
        public List<GioHang> Laygiohang()
        {

            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
     
        public ActionResult Themgiohang(int id, string strURL)
        {
     
            List<GioHang> lstGiohang = Laygiohang();
      
            GioHang SP = lstGiohang.Find(n => n.iidSP == id);
            if (SP == null)
            {
                SP = new GioHang(id);
                lstGiohang.Add(SP);
                return Redirect(strURL);
            }
            else
            {
                SP.iSoluong++;
                return Redirect(strURL);
            }
        }
    
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
    
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
     
        public ActionResult GioHang()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
     
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongtien = TongTien();
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }
       
        public ActionResult Xoa1SP(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iidSP == id);
                return RedirectToAction("ViewGH");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "TDTStore");
            }
            return RedirectToAction("ViewGH");
        }
        //Xóa All Sản phẩm trong giỏ hàng 
        public ActionResult XoaAll()
        {
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "TDTStore");
        }
        //cập nhật thông tin 1 sản phẩm trong giỏ hàng
        public ActionResult Capnhat(int id, FormCollection collection)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSoluong"].ToString());
            }
            return RedirectToAction("ViewGH");
        }
        public ActionResult ViewGH()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult GioHangMomo()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult GioHangVNPay()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult Dathang()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Chưa thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
           
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Index", "Sale");

        }
        public ActionResult DH_ThanhToanOnl()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Đã thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("ConfirmPaymentClient", "Sale");

        }
        public ActionResult Xacnhan()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
        /*==========================Thanh Toán Bằng Ví Momo======================================*/
        public ActionResult Payment()
        {
            List<GioHang> gioHangs = Session["GioHang"] as List<GioHang>;

            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOT9Z620211121";
            string accessKey = "kUeGERJbR1MvBZlk";
            string serectkey = "9j8Gc5SJB4Xo6sFw5pDAlbQHPyDJmgpN";
            string orderInfo = (string)Session["tendangnhap"];
            string returnUrl = "https://localhost:44375/Sale/DH_ThanhToanOnl";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Sale/SavePayment"; 

            string amount = gioHangs.Sum(n => n.dThanhtien).ToString(); 
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

           
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
        
            string signature = crypto.signSHA256(rawHash, serectkey);


            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

      
        public ActionResult ConfirmPaymentClient()
        {
           
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Đã thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();

            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
        }
        /*==================================================================================*/
        /*==============================Thanh toán bằng VNPay===============================*/
        public ActionResult PaymentVNPay()
        {
            List<GioHang> gioHangs = Session["GioHang"] as List<GioHang>;
            string tien = gioHangs.Sum(n => n.dThanhtien * 100).ToString();
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.0.0"); 
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", tien); 
            pay.AddRequestData("vnp_BankCode", ""); 
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); 
            pay.AddRequestData("vnp_CurrCode", "VND"); 
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn"); 
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); 
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); 
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); 
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); 
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; 

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); 

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        
                        DonDatHang ddh = new DonDatHang();
                        ThanhVien Cus = (ThanhVien)Session["tk"];
                        List<GioHang> gh = Laygiohang();

                        ddh.NguoiNhan = Cus.HoTen;
                        ddh.SDT = Cus.DienthoaiKH;
                        ddh.DiaChiNhan = Cus.DiachiKH;
                        ddh.NgayDat = DateTime.Now;
                        ddh.NgayGiao = DateTime.Today.AddDays(4);

                        ddh.idKH = Cus.id;
                        ddh.Datthanhtoan = "Đã thanh toán";
                        ddh.idTrangThai = int.Parse("1");
                        data.DonDatHangs.InsertOnSubmit(ddh);
                        data.SubmitChanges();
                        
                        foreach (var item in gh)
                        {
                            CT_DDH ctddh = new CT_DDH();
                            ctddh.idDDH = ddh.idDDH;
                            ctddh.idSP = item.iidSP;
                            ctddh.SL = item.iSoluong;
                            ctddh.DonGia = item.dThanhtien;
                            data.CT_DDHs.InsertOnSubmit(ctddh);
                        }
                        data.SubmitChanges();
                        Session["Giohang"] = null;
                        return RedirectToAction("ConfirmPaymentClient", "Sale");

                    }
                    else
                    {
                        
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        /*==================================================================================*/
        public ActionResult LichSuMH(int id)
        {
            if (Session["tk"] == null)
                return RedirectToAction("LogIn", "NguoiDung");
            else
            {
                var LS = from ls in data.CT_DDHs.OrderByDescending(n => n.idDDH) where ls.DonDatHang.idKH == id select ls;
                return View(LS);
            }
        }
         
    }
}