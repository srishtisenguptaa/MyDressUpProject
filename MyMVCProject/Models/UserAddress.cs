using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMVCProject.Models
{
    public class UserAddress
    {

       
        public int AddressId { get; set; }
        public int UserId { get; set; }
        //  // public string FullName { get; set; }
        ////   public string Phone { get; set; }
        public string? Pincode { get; set; }
        public string? District { get; set; }
        public string? State { get; set; }
        public string Country { get; set; } = "India";
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FullAddress => $"{Address},  {State}, {State} - {Pincode}";
    }
}
