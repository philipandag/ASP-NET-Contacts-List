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
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        public CategoriesController(ApplicationDbContext db)
        {
            _database = db;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ContactCategoryDTO>> GetCategories()
        {
            var categories = _database.ContactCategories.Select(x => new ContactCategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                WildcardCategory = x.WildcardCategory
            });

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContactCategoryDTO> GetCategory(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }
            var category = _database.ContactCategories.FirstOrDefault(x => x.Id == id);
            if (category == null) // contact not found
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpGet("{id:int}/Subcategories")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ContactCategoryDTO>> GetSubCategoriesOf(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }
            var category = _database.ContactCategories.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var categories = _database.ContactSubcategories.Where(x => x.SubcategoryFor == category);
            var categoriesdto = categories.Select(x => new ContactSubcategoryDTO
            {
                Id = x.Id,
                Name = x.Name
            });

            return Ok(categories);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ContactCategoryDTO> CreateCategory([FromBody] ContactCategoryDTO categoryDTO)
        {
            // check if category already exists
            if (_database.ContactCategories.FirstOrDefault(u => u.Name.ToLower() == categoryDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Error", "Category with this name already exists");
                return Conflict(ModelState);
            }
            if (categoryDTO == null) // invalid contact
            {
                return BadRequest(categoryDTO);
            }
            if (categoryDTO.Id > 0) // id should not be set
            {
                return BadRequest(categoryDTO);
            }

            ContactCategory model = new()
            {
                Id= categoryDTO.Id,
                Name = categoryDTO.Name
            };



            _database.ContactCategories.Add(model);
            _database.SaveChanges(); // commit database transaction

            return CreatedAtAction(nameof(CreateCategory), new { id = categoryDTO.Id }, categoryDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult DeleteCategory(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }

            // find the contact to delete
            var category = _database.ContactCategories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _database.ContactCategories.Remove(category);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult UpdateCategory(int id, [FromBody] ContactCategoryDTO categoryDTO)
        {
            // check if the request is valid
            if (categoryDTO == null || id != categoryDTO.Id || categoryDTO.Id < 1)
            {
                return BadRequest();
            }

            // find the category to update
            var contact = _database.ContactCategories.AsNoTracking().FirstOrDefault(u => u.Id == categoryDTO.Id);
            if (contact == null)
            {
                return NotFound();
            }

            ContactCategory model = new()
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name
            };

            _database.ContactCategories.Update(model);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }
    }
}
