using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Owin;
using Owin;
using System.Web.Hosting;

[assembly: OwinStartup(typeof(BookingTicketFilm_NamGiang.Startup))]

namespace BookingTicketFilm_NamGiang
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Khởi tạo Firebase chỉ một lần
            if (FirebaseApp.DefaultInstance == null)
            {
                // Sử dụng HostingEnvironment.MapPath để tạo đường dẫn tuyệt đối từ đường dẫn tương đối
                string pathToServiceAccountKey = HostingEnvironment.MapPath("~/firebase-config.json");

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToServiceAccountKey),
                });
            }
        }
    }
}
