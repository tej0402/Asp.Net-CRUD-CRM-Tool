using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Customer.Models
{
    
    public class FollowUpModel
    {
        public string FirstName { get; set; }
        public int? FollowUpId { get; set; }

        [Required(ErrorMessage = "FollowUp Date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Followup Date")]
        public DateTime? FollowUpDate { get; set; }

        [DisplayName("Followup Type")]
        [Required(ErrorMessage = "Choose FollowUpType")]
        public int FollowUpType { get; set; }

        [Required(ErrorMessage = "Comments is required.")]
        [StringLength(100, ErrorMessage = "Comments cannot be longer than 100 characters.")]
        public string Comments { get; set; }

        [Required(ErrorMessage = "Choose Status")]
        public int Status { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Next Followup")]
        public DateTime? NextFollowUpDate { get; set; }

        public int Cid { get; set; }

        public List<FollowUpModel> cusDetailsList { get; set; }
        public object FC { get; internal set; }
    }
    //public class CustomerModel
    //{
        
    //}
}