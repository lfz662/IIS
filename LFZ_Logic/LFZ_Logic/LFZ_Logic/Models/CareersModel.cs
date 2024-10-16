using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LFZ_Logic.Models
{
    public class CareersModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your mobile number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter email-Id")]
        [EmailAddress(ErrorMessage = "Please enter valid email-Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please enter your area of interest")]
        public string Areaofinterest { get; set; }
        
        [Required(ErrorMessage = "Please enter your educational background")]
        public string EducationalBackground { get; set; }

        [Required(ErrorMessage = "Please upload file")]
        public HttpPostedFileBase UploadFile { get; set; }
    }
}