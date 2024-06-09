using ASP_NET_Contacts_List.Data;
using ASP_NET_Contacts_List.Models;
using ASP_NET_Contacts_List.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_NET_Contacts_List.Controllers
{
    [Route("api/Contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {

        private readonly ApplicationDbContext _database;
        public ContactsController(ApplicationDbContext db)
        {
            _database = db;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ContactDTO>> GetContacts()
        {
            var contacts = _database.Contacts.Select(x => new ContactDTO
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                DateOfBirth = x.DateOfBirth,
                MainCategory = x.MainCategory != null ? (new ContactCategoryDTO { Id = x.MainCategory.Id, Name = x.MainCategory.Name}) : null,
                SubCategory = x.SubCategory != null ? (new ContactSubcategoryDTO { Id = x.SubCategory.Id, Name = x.SubCategory.Name}) : null
            });
            return Ok(contacts);
        }

        //authentication endpoint, create cookie session
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] ContactLoginCredentialsDTO contactLoginCredentialsDTO)
        {
            var contact = _database.Contacts.FirstOrDefault(x => x.Email == contactLoginCredentialsDTO.Email);
            if (contact == null)
            {
                return Unauthorized();
            }

            // generate password hash from given password to compare with hash in database
            var passwordHasher = new PasswordHasher<Contact>();
            var result = passwordHasher.VerifyHashedPassword(contact, contact.PasswordHash, contactLoginCredentialsDTO.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized();
            }

            // create cookie session
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, contact.Email) },
                CookieAuthenticationDefaults.AuthenticationScheme))
            );

            return Ok();
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }


        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContactLoginCredentialsDTO> GetContact(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }
            var contact = _database.Contacts.FirstOrDefault(x => x.Id == id);
            if (contact == null) // contact not found
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ContactDTO> CreateContact([FromBody] ContactDTO contactDTO)
        {
            // check if contact already exists
            if (_database.Contacts.FirstOrDefault(u => u.Email.ToLower() == contactDTO.Email.ToLower()) != null)
            {
                ModelState.AddModelError("Error", "Contact with this email already exists");
                return Conflict(ModelState);
            }
            if (contactDTO == null) // invalid contact
            {
                return BadRequest(contactDTO);
            }
            if (contactDTO.Id > 0) // id should not be set
            {
                return BadRequest(contactDTO);
            }

            //Create password hash for the contact
            var passwordHasher = new PasswordHasher<Contact>();
            var hashed = passwordHasher.HashPassword(null, contactDTO.Password);

            //find categories and subcategories in database to make sure they exist
            var category = _database.ContactCategories.FirstOrDefault(c => c.Name == contactDTO.MainCategory.Name);
            if (category == null)
            {
                ModelState.AddModelError("Error", "Category does not exist");
                return BadRequest(ModelState);
            }

            var subCategory = _database.ContactSubCategories.FirstOrDefault(c => c.Name == contactDTO.SubCategory.Name);
            if (subCategory == null)
            {
                ModelState.AddModelError("Error", "SubCategory does not exist");
                return BadRequest(ModelState);
            }




            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Email = contactDTO.Email,
                PasswordHash = hashed,
                MainCategory = category,
                SubCategory = subCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth
            };


            _database.Contacts.Add(model);
            _database.SaveChanges(); // commit database transaction

            return CreatedAtAction(nameof(GetContact), new { id = contactDTO.Id }, contactDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteContact")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult DeleteContact(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }

            // find the contact to delete
            var contact = _database.Contacts.FirstOrDefault(u => u.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            _database.Contacts.Remove(contact);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateContact")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult UpdateContact(int id, [FromBody] ContactDTO contactDTO)
        {
            // check if the request is valid
            if (contactDTO == null || id != contactDTO.Id || contactDTO.Id < 1)
            {
                return BadRequest();
            }

            // find the contact to update
            var contact = _database.Contacts.AsNoTracking().FirstOrDefault(u => u.Id == contactDTO.Id);
            if (contact == null)
            {
                return NotFound();
            }

            // generate password hash from given password
            var passwordHasher = new PasswordHasher<Contact>();
            var hashed = passwordHasher.HashPassword(null, contactDTO.Password);

            // find categories and subcategories in database to make sure they exist
            var category = _database.ContactCategories.FirstOrDefault(c => c.Name == contactDTO.MainCategory.Name);
            if (category == null)
            {
                ModelState.AddModelError("Error", "Category does not exist");
                return BadRequest(ModelState);
            }

            var subCategory = _database.ContactSubCategories.FirstOrDefault(c => c.Name == contactDTO.SubCategory.Name);
            if (subCategory == null)
            {
                ModelState.AddModelError("Error", "SubCategory does not exist");
                return BadRequest(ModelState);
            }



            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Email = contactDTO.Email,
                PasswordHash = hashed,
                MainCategory = category,
                SubCategory = subCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth
            };

            _database.Contacts.Update(model);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }
    }
}
