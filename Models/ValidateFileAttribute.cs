using System;

namespace Customer.Models
{
    internal class ValidateFileAttribute : Attribute
    {
        public string ErrorMessage { get; set; }
    }
}