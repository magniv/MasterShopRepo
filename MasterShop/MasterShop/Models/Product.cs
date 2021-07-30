using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        [StringLength(200, MinimumLength = 1)]
        [Required(ErrorMessage = "You must input product name")]
        public string Name { get; set; }

        [Range(0, float.MaxValue)]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        [Display(Name = "Product Description")]
        [StringLength(200)]
        [Required(ErrorMessage = "You must input product description")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "You must select category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Image Url")]
        [Required(ErrorMessage = "You must input image url")]
        public string ImageUrl { get; set; }

        public ICollection<ProductOrder> ProductOrders { get; set; }

    }
}
