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
                Subcategory = x.Subcategory != null ? (new ContactSubcategoryDTO { Id = x.Subcategory.Id, Name = x.Subcategory.Name}) : null
            });
            return Ok(contacts);
        }

        //authentication endpoint, create cookie session
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] ContactLoginCredentialsDTO contactLoginCredentialsDTO)
        {
            var contact = _database.Contacts.Include(c=>c.MainCategory).Include(c=>c.Subcategory).FirstOrDefault(x => x.Email == contactLoginCredentialsDTO.Email);
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

            var user = new ContactDTO
            {
                Id = contact.Id,
                Name = contact.Name,
                Surname = contact.Surname,
                Email = contact.Email,
                DateOfBirth = contact.DateOfBirth,
                MainCategory = contact.MainCategory != null ? (new ContactCategoryDTO { Id = contact.MainCategory.Id, Name = contact.MainCategory.Name }) : null,
                Subcategory = contact.Subcategory != null ? (new ContactSubcategoryDTO { Id = contact.Subcategory.Id, Name = contact.Subcategory.Name }) : null
            };

            return Ok(user);
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
        public ActionResult<ContactDTO> GetContact(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }
            var contact = _database.Contacts.Where(x => x.Id == id).Include(c=>c.MainCategory).Include(c=>c.Subcategory).Select(x => new ContactDTO
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                DateOfBirth = x.DateOfBirth,
                MainCategory = x.MainCategory != null ? (new ContactCategoryDTO { Id = x.MainCategory.Id, Name = x.MainCategory.Name }) : null,
                Subcategory = x.Subcategory != null ? (new ContactSubcategoryDTO { Id = x.Subcategory.Id, Name = x.Subcategory.Name }) : null,
                PhoneNumber = x.PhoneNumber

            }).FirstOrDefault();
            
            if (contact == null) // contact not found
            {
                return NotFound();
            }

            // Create a new ContactDTO object with the data from the database
            var contactDTO = new ContactDTO
            {
                Id = contact.Id,
                Name = contact.Name,
                Surname = contact.Surname,
                Email = contact.Email,
                Password = "",
                MainCategory = contact.MainCategory != null ? (new ContactCategoryDTO { Id = contact.MainCategory.Id, Name = contact.MainCategory.Name }) : null,
                Subcategory = contact.Subcategory != null ? (new ContactSubcategoryDTO { Id = contact.Subcategory.Id, Name = contact.Subcategory.Name }) : null,
                PhoneNumber = contact.PhoneNumber,
                DateOfBirth = contact.DateOfBirth,
            };

            return Ok(contactDTO);
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
            if(contactDTO.MainCategory == null || contactDTO.Subcategory == null)
            {
                ModelState.AddModelError("Error", "Category or Subcategory is missing");
                return BadRequest(ModelState);
            }

            var category = _database.ContactCategories.FirstOrDefault(c => c.Name == contactDTO.MainCategory.Name);
            if (category == null)
            {
                ModelState.AddModelError("Error", "Category does not exist");
                return BadRequest(ModelState);
            }

            var subCategory = _database.ContactSubcategories.FirstOrDefault(c => c.Name == contactDTO.Subcategory.Name);

            if (subCategory == null)
            {
                if (category.WildcardCategory == true)
                {
                    ContactSubcategory newsSubDirectory = new()
                    {
                        Name = contactDTO.Subcategory.Name,
                        Id = 0,
                        SubcategoryFor = category
                    };
                    _database.ContactSubcategories.Add(newsSubDirectory);
                    _database.SaveChanges();
                    subCategory = newsSubDirectory;
                }
                else
                {
                    ModelState.AddModelError("Error", "Subcategory does not exist");
                    return BadRequest(ModelState);
                }
            }

            Contact model = new()
            {
                Id = contactDTO.Id,
                Name = contactDTO.Name,
                Surname = contactDTO.Surname,
                Email = contactDTO.Email,
                PasswordHash = hashed,
                MainCategory = category,
                Subcategory = subCategory,
                PhoneNumber = contactDTO.PhoneNumber,
                DateOfBirth = contactDTO.DateOfBirth,
            };


            _database.Contacts.Add(model);
            _database.SaveChanges(); // commit database transaction

            return CreatedAtAction(nameof(GetContact), new { id = contactDTO.Id }, contactDTO);
        }

        [HttpDelete("{id:int}")]
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

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize]
        public IActionResult UpdateContact(int id, [FromBody] ContactDTO contactDTO)
        {
            // check if the request is valid
            if (contactDTO == null || id != contactDTO.Id || contactDTO.Id < 1)
            {
                ModelState.AddModelError("Error", "tried to change ID");
                return BadRequest();
            }

            // get the contact to update details
            var contact = _database.Contacts.Where(u => u.Id == contactDTO.Id).Include(c => c.MainCategory).Include(c => c.Subcategory).FirstOrDefault();
            if (contact == null)
            {
                return NotFound();
            }

            // generate password hash from given password or use the old one if password is not given
            string hashed;
            if (contactDTO.Password != "")
            {
                var passwordHasher = new PasswordHasher<Contact>();
                hashed = passwordHasher.HashPassword(null, contactDTO.Password);
            }
            else
            {
                hashed = contact.PasswordHash;
            }

            // Find new category  in database or use the old one if it is not given
            ContactCategory category;
            if(contactDTO.MainCategory == null)
            {
                category = contact.MainCategory;
            }
            else
            {
                category = _database.ContactCategories.FirstOrDefault(c => c.Name == contactDTO.MainCategory.Name);
            }
            if (category == null)
            {
                ModelState.AddModelError("Error", "Category does not exist");
                return BadRequest(ModelState);
            }

            // Find new subcategory  in database or use the old one if it is not given
            ContactSubcategory subcategory;
            if(contactDTO.Subcategory == null)
            {
                subcategory = contact.Subcategory;
            }
            else
            {
                subcategory = _database.ContactSubcategories.FirstOrDefault(c => c.Name == contactDTO.Subcategory.Name);
            }
            if (subcategory == null)
            {
                ModelState.AddModelError("Error", "Subcategory does not exist");
                return BadRequest(ModelState);
            }

            var checkemail = _database.Contacts.FirstOrDefault(u => u.Email.ToLower() == contactDTO.Email.ToLower());
            if (checkemail != null && checkemail.Id != contactDTO.Id)
            {
                ModelState.AddModelError("Error", "Contact with this email already exists");
                return Conflict(ModelState);
            }

            // update contact details
            contact.Name = contactDTO.Name;
            contact.Surname = contactDTO.Surname;
            contact.Email = contactDTO.Email;
            contact.PasswordHash = hashed;
            contact.MainCategory = category;
            contact.Subcategory = subcategory;
            contact.PhoneNumber = contactDTO.PhoneNumber;
            contact.DateOfBirth = contactDTO.DateOfBirth;




            _database.Contacts.Update(contact);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }
    }
}
