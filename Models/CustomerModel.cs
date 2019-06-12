using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Customer.Models
{
    public class CustomerModel
    {
        public int? CustomerId { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9 ]+$",ErrorMessage ="Please Fill Without Spaces")]
       [StringLength(10, ErrorMessage = "FirstName cannot be longer than 10 characters.")]
        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9 ]+$", ErrorMessage = "Please Fill Without Spaces")]
        [StringLength(10, ErrorMessage = "LastName cannot be longer than 10 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Choose Gender")]
        public int Gender { get; set; }
       
        [Required(ErrorMessage ="Choose Date Of Birth")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DoB { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Please Enter a Valid Email Address")]
        public string Email { get; set; }
       

        [RegularExpression(@"^[789]\d{9}$", ErrorMessage = "Please Enter a Valid Mobile Number")]
        [Display(Name = "Mobile")]
        public string PhoneNumber { get; set; }

        public List<CustomerModel> cusDetailsList { get; set; }

        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public string Address { get; set; }


        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9 ]+$", ErrorMessage = "Please Fill Without Spaces")]
        [StringLength(20, ErrorMessage = "City Name cannot be longer than 20 characters.")]
        public string City { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9 ]+$", ErrorMessage = "Please Fill Without Spaces")]
        [StringLength(30, ErrorMessage = "State Name cannot be longer than 30 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Please Provide Valid Pincode")]
        public int? Pincode { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public int? FollowUpCount { get; set;}
        
        public string ImagePath { get; set; }

        
        [DisplayName("Upload Image")]
        public HttpPostedFileBase ImageFile { get; set; }

    }
}