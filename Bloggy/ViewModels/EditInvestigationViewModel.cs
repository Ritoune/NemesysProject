using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bloggy.ViewModels
{
    public class EditInvestigationViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A description is required")]
        [StringLength(50)]
        [Display(Name = "Description of investigation")]
        public string DescriptionOfInvestigation { get; set; }
        //[Display(Name = "Spotted Date :")]
        //public DateTime DateOfAction { get; set; }
        //[Required(ErrorMessage = "Report description is required")]
        //[StringLength(1500, ErrorMessage = "Report description cannot be longer than 1500 characters")]
        //[Display(Name = "Description")]
        //public string UserId { get; set; }
        public int BlogPostId { get; set; }
        //Property used to bind user selection
        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }
        public bool HasInvestigation { get; set; }
        public List<StatusViewModel> StatusList { get; set; }
        //Property used solely to populate drop down
        public List<BlogPostViewModel> BlogPostList { get; set; }
    }
}
