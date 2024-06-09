using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ASP_NET_Contacts_List.Models.DTO
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public ContactCategoryDTO? MainCategory { get; set; } = null;
        public ContactSubcategoryDTO? SubCategory { get; set; } = null;
        public string PhoneNumber { get; set; } = string.Empty;

        [AllowNull]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
