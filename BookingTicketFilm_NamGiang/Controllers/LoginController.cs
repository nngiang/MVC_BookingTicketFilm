using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Firebase.Auth;
using BookingTicketFilm_NamGiang.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace BookingTicketFilm_NamGiang.Controllers
{
    //[OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)] // xử lý lỗi antitoken ko khớp
    public class LoginController : Controller
    {
        private static string ApiKey = "AIzaSyBrJSxbb82yLfFAd-5MFa8K0965e5HXq6E";
        private static string Bucket = "bookingticketfilm.appspot.com";
        //private static string RecaptchaSecretKey = "6Lf-oAgqAAAAAI8LHgCQPrjBBqsud8H888KiFawV";
        private static string EmailSender = "namgiangkt1010@gmail.com"; // Địa chỉ email người gửi
        private static string EmailPassword = "epxlmgjjhwusoded"; // Mật khẩu email người gửi (Mật khẩu ứng dụng toàn quyền)
        private static int ConfirmationCodeValidityInSeconds = 30; //giây

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult VerifyConfirmationCode()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserData model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var authLink = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                string token = authLink.FirebaseToken;

                if (!string.IsNullOrEmpty(token))
                {
                    string emailAddress = Request.Form["Email"];

                    string confirmationCode = GenerateConfirmationCode();
                    string emailContent = $"Mã xác nhận của bạn là: {confirmationCode}. Hãy sử dụng mã này để xác nhận đăng nhập vào Website BookingTicketFilm.";

                    bool emailSent = SendEmail(model.Email, "Xác nhận đăng nhập - BookingTicketFilm", emailContent);

                    if (emailSent)
                    {
                        // Lưu mã xác nhận và datetime vào TempData để chuyển sang trang nhập mã
                        TempData["ConfirmationCode"] = confirmationCode;
                        TempData["Email"] = model.Email;
                        TempData["ConfirmationCodeTimestamp"] = DateTime.Now;

                        return RedirectToAction("VerifyConfirmationCode", "Login");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to send email.";
                        return View();
                    }

                    //TempData["SuccessMessage"] = "Login successful!";
                    //return RedirectToAction("Home_User", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Sai tài khoản hoặc mật khẩu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Sai tài khoản hoặc mật khẩu.";
                return View();
            }
        }

        //Create randomVeritify
        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            int codeLength = random.Next(6, 8); //Độ dài mã xác nhận từ 6 đến 8 chữ số
            string confirmationCode = "";

            for (int i = 0; i < codeLength; i++)
            {
                confirmationCode += random.Next(0, 10); //Chọn ngẫu nhiên từ 0 đến 9
            }

            return confirmationCode;
        }

        //Phương thức gửi email
        private bool SendEmail(string emailAddress, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(EmailSender, EmailPassword);
                    client.EnableSsl = true;

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(EmailSender);
                        message.To.Add(emailAddress);
                        message.Subject = subject;
                        message.Body = body;

                        client.Send(message);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Xử lý khi gửi email thất bại
                return false;
            }
        }

        // Action hiển thị trang nhập mã xác nhận
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyConfirmationCode(string confirmationCode)
        {
            try
            {
                // Lấy mã xác nhận và thời gian đếm ngược từ TempData
                string storedConfirmationCode = TempData["ConfirmationCode"] as string;
                string email = TempData["Email"] as string;
                DateTime? timestamp = TempData["ConfirmationCodeTimestamp"] as DateTime?;

                // Đảm bảo giữ mã xác nhận trong TempData cho lần xác nhận sau nếu không hợp lệ
                TempData.Keep("ConfirmationCode");
                TempData.Keep("Email");
                TempData.Keep("ConfirmationCodeTimestamp");

                if (string.IsNullOrEmpty(storedConfirmationCode) || !timestamp.HasValue)
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập mã xác nhận.";
                    return View("VerifyConfirmationCode");
                }

                // Kiểm tra thời gian hiệu lực của mã xác nhận
                if ((DateTime.Now - timestamp.Value).TotalSeconds > ConfirmationCodeValidityInSeconds)
                {
                    TempData.Remove("ConfirmationCode"); //xoá mã khi hết hạn
                    ViewBag.ErrorMessage = "Mã xác nhận đã hết hiệu lực.";
                    return View("VerifyConfirmationCode");
                }

                if (confirmationCode != storedConfirmationCode)
                {
                    ViewBag.ErrorMessage = "Mã xác nhận không hợp lệ.";
                    return View("VerifyConfirmationCode");
                }

                // Xử lý khi mã xác nhận hợp lệ, ví dụ: cập nhật trạng thái đăng nhập cho người dùng

                TempData.Remove("ConfirmationCode"); // Xóa mã xác nhận sau khi xác nhận thành công
                TempData.Remove("Email"); // Xóa email sau khi xác nhận thành công
                TempData.Remove("ConfirmationCodeTimestamp"); // Xóa thời gian tạo mã sau khi xác nhận thành công
                return RedirectToAction("Home_User", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình xác nhận mã.";
                return View("VerifyConfirmationCode");
            }
        }

    }
}
