using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        [Required]
        public String ISBN { get; set; }
        [Required]
        public String Author { get; set; }
        [Required]
        [Display (Name = "List Price")]
        [Range(1,1000)]
        public Double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        public Double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public Double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public Double Price100 { get; set; }
        public int CategoryId {  get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category category { get; set; }
        [ValidateNever]
        public string ImgURL {  get; set; }
    }
}
