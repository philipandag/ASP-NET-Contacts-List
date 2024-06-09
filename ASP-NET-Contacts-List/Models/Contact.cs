using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ASP_NET_Contacts_List.Models
{
    public class Contact : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;

        public override string PasswordHash { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        [AllowNull]
        public ContactCategory? MainCategory { get; set; }

        [AllowNull]
        public ContactSubCategory? SubCategory { get; set; }

        [AllowNull]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
