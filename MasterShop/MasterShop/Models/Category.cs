using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 1)]
        [Required(ErrorMessage = "You must input category name")]
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}
