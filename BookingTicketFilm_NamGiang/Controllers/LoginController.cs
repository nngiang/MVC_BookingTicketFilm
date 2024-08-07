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
    public class LoginController : Controller
    {
        private static string ApiKey = "AIzaSyBrJSxbb82yLfFAd-5MFa8K0965e5HXq6E";
        private static string Bucket = "bookingticketfilm.appspot.com";
        private static string EmailSender = "namgiangkt1010@gmail.com"; // Địa chỉ email người gửi
        private static string EmailPassword = "epxlmgjjhwusoded"; // Mật khẩu email người gửi (Mật khẩu ứng dụng toàn quyền)
        private static int ConfirmationCodeValidityInSeconds = 30; // giây

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // GET: VerifyConfirmationCode
        [AllowAnonymous]
        public ActionResult VerifyConfirmationCode()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserData model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var authLink = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                string token = authLink.FirebaseToken;

                if (!string.IsNullOrEmpty(token))
                {
                    string confirmationCode = GenerateConfirmationCode();
                    string emailContent = $"Mã xác nhận của bạn là: {confirmationCode}. Hãy sử dụng mã này để xác nhận đăng nhập vào Website BookingTicketFilm.";

                    bool emailSent = SendEmail(model.Email, "Xác nhận đăng nhập - BookingTicketFilm", emailContent);

                    if (emailSent)
                    {
                        // Lưu mã xác nhận và datetime vào TempData để chuyển sang trang nhập mã
                        TempData["ConfirmationCode"] = confirmationCode;
                        TempData["Email"] = model.Email;
                        TempData["ConfirmationCodeTimestamp"] = DateTime.Now;

                        return RedirectToAction("VerifyConfirmationCode");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to send email.";
                        return View();
                    }
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

        // Create random verification code
        private string GenerateConfirmationCode()
        {
            Random random = new Random();
            int codeLength = random.Next(6, 8); // Độ dài mã xác nhận từ 6 đến 8 chữ số
            string confirmationCode = "";

            for (int i = 0; i < codeLength; i++)
            {
                confirmationCode += random.Next(0, 10); // Chọn ngẫu nhiên từ 0 đến 9
            }

            return confirmationCode;
        }

        // Send email method
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
                // Handle email sending failure
                return false;
            }
        }

        // POST: VerifyConfirmationCode
        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyConfirmationCode(string confirmationCode)
        {
            try
            {
                // Get stored confirmation code and timestamp from TempData
                string storedConfirmationCode = TempData["ConfirmationCode"] as string;
                string email = TempData["Email"] as string;
                DateTime? timestamp = TempData["ConfirmationCodeTimestamp"] as DateTime?;

                // Ensure the confirmation code is kept in TempData for subsequent validation
                TempData.Keep("ConfirmationCode");
                TempData.Keep("Email");
                TempData.Keep("ConfirmationCodeTimestamp");

                if (string.IsNullOrEmpty(storedConfirmationCode) || !timestamp.HasValue)
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập mã xác nhận.";
                    return View();
                }

                // Check the validity of the confirmation code
                if ((DateTime.Now - timestamp.Value).TotalSeconds > ConfirmationCodeValidityInSeconds)
                {
                    TempData.Remove("ConfirmationCode"); // Remove expired code
                    ViewBag.ErrorMessage = "Mã xác nhận đã hết hiệu lực.";
                    return View();
                }

                if (confirmationCode != storedConfirmationCode)
                {
                    ViewBag.ErrorMessage = "Mã xác nhận không hợp lệ.";
                    return View();
                }

                // Process successful verification, e.g., update user login status

                TempData.Remove("ConfirmationCode"); // Remove code after successful verification
                TempData.Remove("Email"); // Remove email after successful verification
                TempData.Remove("ConfirmationCodeTimestamp"); // Remove timestamp after successful verification
                return RedirectToAction("Home_User", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình xác nhận mã.";
                return View();
            }
        }
    }
}
