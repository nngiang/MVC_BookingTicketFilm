using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookingTicketFilm_NamGiang.Models
{
    public class BookingTicketFilmEntities : DbContext
    {
        // Khai báo DbSet cho các bảng trong cơ sở dữ liệu
        public DbSet<UserData> Users { get; set; }

        // Constructor để chỉ định chuỗi kết nối
        public BookingTicketFilmEntities() : base("name=TicketFilmEntities")
        {
        }
    }
}