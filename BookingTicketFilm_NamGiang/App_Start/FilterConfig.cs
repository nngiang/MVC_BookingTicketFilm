using System.Web;
using System.Web.Mvc;

namespace BookingTicketFilm_NamGiang
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
