using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterShop.Models
{

    public enum userType
    {
        Admin, 
        Customer
    }
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must input an email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password required 8 min length")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password required 8 min length")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must input a full name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "You must select gender")]
        public string Gender { get; set; }

        public userType Type { get; set; }
    }
}
