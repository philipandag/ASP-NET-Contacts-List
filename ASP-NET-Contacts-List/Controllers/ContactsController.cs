using Microsoft.AspNetCore.Mvc;
using ASP_NET_Contacts_List.Models;
namespace ASP_NET_Contacts_List.Controllers
{
    [Route("api/Contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Contact> GetContacts()
        {
            return new List<Contact>
            {
                new Contact { Id = 1, Name = "John Doe", Password = "123456" },
                new Contact { Id = 2, Name = "Jane Doe", Password = "123" }
            };
        }
    }
}
