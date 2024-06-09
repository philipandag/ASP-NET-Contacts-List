using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ASP_NET_Contacts_List.Models
{
    public class ContactSubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        [AllowNull]
        public ContactCategory SubcategoryFor { get; set; }

        public IEnumerable<Contact> ContactsWithSubcategory { get;}
    }
}
