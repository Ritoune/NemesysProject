using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bloggy.ViewModels
{
    public class EditBlogPostViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A title is required")]
        [StringLength(50)]
        [Display(Name = "Report heading")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Report description is required")]
        [StringLength(1500, ErrorMessage = "Report description cannot be longer than 1500 characters")]
        [Display(Name = "Description")]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        [Display(Name = "Featured Image")]
        public IFormFile ImageToUpload { get; set; } //used only when submitting form
        [Display(Name = "Report type of hazard")]

        //Property used to bind user selection
        [Required(ErrorMessage = "Type of hazard is required")]
        public int CategoryId { get; set; }
        [Display(Name = "Location :")]
        public string Location { get; set; }
        [Display(Name = "TypeOfHazard :")]
        public string TypeOfHazard { get; set; }



        //Property used solely to populate drop down
        public List<CategoryViewModel> CategoryList { get; set; }
    }
}
