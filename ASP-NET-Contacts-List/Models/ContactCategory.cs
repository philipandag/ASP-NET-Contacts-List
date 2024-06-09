using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_NET_Contacts_List.Models
{
    public class ContactCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public IEnumerable<ContactSubCategory> SubCategories { get; set; }
        public IEnumerable<Contact> ContactsWithCategory { get; set; }
    }
}
