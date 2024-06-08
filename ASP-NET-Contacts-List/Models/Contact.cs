﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ASP_NET_Contacts_List.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // auto increment key
        public int Id { get; set; }

        [AllowNull]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [AllowNull]
        public string Category { get; set; }

        [AllowNull]
        public string SubCategory { get; set; }

        [AllowNull]
        public string PhoneNumber { get; set; }

        [AllowNull]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
