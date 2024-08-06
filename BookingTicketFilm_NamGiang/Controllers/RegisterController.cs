using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using BookingTicketFilm_NamGiang.Models;
using Firebase.Auth;

namespace BookingTicketFilm_NamGiang.Controllers
{
    public class RegisterController : Controller
    {
        private static string ApiKey = "AIzaSyBrJSxbb82yLfFAd-5MFa8K0965e5HXq6E";
        private static string Bucket = "bookingticketfilm.appspot.com";

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserData model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Registration successful! Please verify your email then login.";

                // Chuyển hướng đến chính trang đăng ký để hiển thị SweetAlert2
                return RedirectToAction("Register");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (this.Request.IsAuthenticated)
                {
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return this.View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
