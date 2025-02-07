﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketFilm_NamGiang.Models
{
    [Table("Users")]
    public class UserDB
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
