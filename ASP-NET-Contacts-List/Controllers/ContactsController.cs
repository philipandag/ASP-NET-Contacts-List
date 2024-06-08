using Microsoft.AspNetCore.Mvc;
using ASP_NET_Contacts_List.Models;
using ASP_NET_Contacts_List.Models.DTO;
using System.Reflection.Metadata.Ecma335;
using ASP_NET_Contacts_List.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authentication;

namespace ASP_NET_Contacts_List.Controllers
{
    [Route("api/Contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly ApplicationDbContext _database;
        public ContactsController(ILogger<ContactsController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _database = db;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ContactDTO>> GetContacts()
        {
            _logger.LogInformation("Getting all contacts");
            return Ok(_database.Contacts);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContactDetailsDTO> GetContact(int id)
        {
            if(id < 1) // ids start from 1
            {
                _logger.LogInformation("Tried to get Contact with invalid id (" + id + ")");
                return BadRequest();
            }
            var contact = _database.Contacts.FirstOrDefault(x => x.Id == id);
            if(contact == null) // contact not found
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ContactDetailsDTO> CreateContact([FromBody] ContactDetailsDTO contactDTO)
        {
            // check if contact already exists
            if(_database.Contacts.FirstOrDefault(u=>u.Name.ToLower() == contactDTO.Name.ToLower())!= null)
            {
                ModelState.AddModelError("Error", "Contact already exists");
                return Conflict(ModelState);
            }
            if(contactDTO == null) // invalid contact
            {
                return BadRequest(contactDTO);
            }
            if(contactDTO.Id > 0) // id should not be set
            {
                return BadRequest(contactDTO);
            }

            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Email = contactDTO.Email,
                Password = contactDTO.Password,
                Category = contactDTO.Category,
                SubCategory = contactDTO.SubCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth
            };
            _database.Contacts.Add(model);
            _database.SaveChanges(); // commit database transaction

            return CreatedAtAction(nameof(GetContact), new { id = contactDTO.Id }, contactDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name ="DeleteContact")]
        public IActionResult DeleteContact(int id)
        {
            if(id < 1) // ids start from 1
            {
                return BadRequest();
            }

            // find the contact to delete
            var contact = _database.Contacts.FirstOrDefault(u => u.Id == id);
            if(contact == null)
            {
                return NotFound();
            }
            _database.Contacts.Remove(contact);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateContact")]
        public IActionResult UpdateContact(int id, [FromBody] ContactDTO contactDTO)
        {
            // check if the request is valid
            if(contactDTO == null || id != contactDTO.Id || contactDTO.Id < 1)
            {
                return BadRequest();
            }

            // find the contact to update
            var contact = _database.Contacts.FirstOrDefault(u => u.Id == contactDTO.Id);
            if(contact == null)
            {
                return NotFound();
            }

            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Email = contactDTO.Email,
                Password = contactDTO.Password,
                Category = contactDTO.Category,
                SubCategory = contactDTO.SubCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth
            };

            _database.Contacts.Update(model);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }


        [HttpPatch("{id:int}", Name = "PartiallyUpdateContact")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PartiallyUpdateContact(int id, JsonPatchDocument<ContactDetailsDTO> patchDTO)
        {
            if(patchDTO == null || id < 1) // ids start from 1
            {
                return BadRequest();
            }

            // find the contact to update
            var contact = _database.Contacts.FirstOrDefault(u => u.Id == id);

            ContactDetailsDTO contactDTO = new()
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Password = contact.Password,
                Category = contact.Category,
                SubCategory = contact.SubCategory,
                PhoneNumber = contact.PhoneNumber,
                DateOfBirth = contact.DateOfBirth
            };

            if(contact == null)
            {
                return NotFound();
            }
            
            patchDTO.ApplyTo(contactDTO, ModelState);

            // check if the patch was successful
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Email = contactDTO.Email,
                Password = contactDTO.Password,
                Category = contactDTO.Category,
                SubCategory = contactDTO.SubCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth
            };
            _database.Contacts.Update(model);
            _database.SaveChanges(); // commit database transaction


            return NoContent();
        }
    }
}
