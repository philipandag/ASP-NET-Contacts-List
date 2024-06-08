using System.ComponentModel.DataAnnotations;

namespace ASP_NET_Contacts_List.Models.DTO
{
    public class ContactDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
