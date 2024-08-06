using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace BookingTicketFilm_NamGiang.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly string ApiKey = "AIzaSyBrJSxbb82yLfFAd-5MFa8K0965e5HXq6E"; //apikey firebase

        // GET: ResetPassword
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: SendPasswordResetEmail
        [HttpPost]
        public async Task<ActionResult> SendPasswordResetEmail(string Email)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={ApiKey}";
                    var content = new StringContent(
                        $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{Email}\"}}",
                        System.Text.Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync(requestUri, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JObject.Parse(responseString);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Password reset email sent successfully." });
                    }
                    else
                    {
                        var errorMessage = responseJson["error"]?["message"]?.ToString() ?? "Unknown error occurred.";
                        return Json(new { success = false, message = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
