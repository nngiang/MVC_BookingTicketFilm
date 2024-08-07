using System.ComponentModel.DataAnnotations;

namespace BookingTicketFilm_NamGiang.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
