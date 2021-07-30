using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        [Display(Name = "Order Time")]
        public DateTime OrderTime { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "You must add an address")]
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "You must add a phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Sum To Pay")]
        public double SumToPay { get; set; }

        public ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
