using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BookingTicketFilm_NamGiang.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace BookingTicketFilm_NamGiang.Controllers
{
    public class HomeController : Controller
    {
        // Phương thức kết nối đến Firebase
        private IFirebaseClient ConnectToFirebase()
        {
            string authSecret = "VcI7H0Ln1AGu1kzYBa7TMNDqZsYY73yhTizWR7zn"; // Mã token của Firebase
            string basePath = "https://bookingticketfilm-default-rtdb.firebaseio.com/"; // Đường dẫn csdl của Firebase

            // Khởi tạo FirebaseConfig
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = authSecret,
                BasePath = basePath
            };

            // Khởi tạo và trả về FirebaseClient
            return new FireSharp.FirebaseClient(config);
        }

        // Phương thức Index để kiểm tra kết nối đến Firebase
        public ActionResult Index()
        {
            IFirebaseClient client = ConnectToFirebase();

            // Kiểm tra xem kết nối thành công chưa
            if (client != null)
            {
                ViewBag.Message = "Firebase connection established successfully!";
            }
            else
            {
                ViewBag.Message = "Failed to connect to Firebase!";
            }

            // Hiển thị view
            return RedirectToAction("Login", "Login");
        }

        // Action để hiển thị form nhập dữ liệu
        public ActionResult InputData()
        {
            return View();
        }

        // Action hiển thị Home_User
        public ActionResult Home_User()
        {
            return View();
        }

        // Action để xử lý việc gửi dữ liệu lên Firebase
        [HttpPost]
        public ActionResult SubmitData(UserData model)
        {
            IFirebaseClient client = ConnectToFirebase();

            if (client != null)
            {
                try
                {
                    // Gửi dữ liệu lên Firebase
                    PushResponse response = client.Push("data/", model);

                    if (response != null)
                    {
                        TempData["SuccessMessage"] = "You will be directed to OutputData";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to send data to Firebase!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to connect to Firebase!";
            }

            return RedirectToAction("InputData");
        }



        // Action để hiển thị dữ liệu từ Firebase
        public ActionResult OutputData()
        {
            IFirebaseClient client = ConnectToFirebase();

            if (client != null)
            {
                try
                {
                    // Truy vấn dữ liệu từ Firebase
                    FirebaseResponse response = client.Get("data/");
                    if (response.Body != "null")
                    {
                        // Chuyển dữ liệu từ Firebase thành một danh sách các đối tượng UserData
                        var dataFromFirebase = response.ResultAs<Dictionary<string, UserData>>();

                        // Khai báo một danh sách để lưu trữ dữ liệu từ Firebase
                        List<UserData> userDataList = new List<UserData>();

                        // Lặp qua mỗi cặp key-value trong dictionary và thêm dữ liệu vào danh sách
                        foreach (var keyValue in dataFromFirebase)
                        {
                            userDataList.Add(keyValue.Value);
                        }

                        // Truyền danh sách này vào view để hiển thị
                        return View(userDataList);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Failed to connect to Firebase!";
            }

            return View(new List<UserData>());
        }

    }
}
