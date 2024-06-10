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
    [Route("api/Subcategories")]
    [ApiController]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        public SubcategoriesController(ApplicationDbContext db)
        {
            _database = db;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ContactSubcategoryDTO>> GetCategories()
        {
            var categories = _database.ContactSubcategories.Select(x => new ContactSubcategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                SubcategoryForId = x.SubcategoryFor.Id
            });

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContactSubcategoryDTO> GetSubcategories(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }
            var category = _database.ContactSubcategories.FirstOrDefault(x => x.Id == id);
            if (category == null) // contact not found
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ContactSubcategoryDTO> CreateSubcategory([FromBody] ContactSubcategoryDTO categoryDTO)
        {
            // check if category already exists
            if (_database.ContactSubcategories.FirstOrDefault(u => u.Name.ToLower() == categoryDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Error", "Subcategory with this name already exists");
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

            if (categoryDTO.SubcategoryForId == 0)
            {
                ModelState.AddModelError("Error", "Subcategory's main category should be set");
                return BadRequest(ModelState);
            }

            var category = _database.ContactCategories.FirstOrDefault(x => x.Id == categoryDTO.SubcategoryForId);
            if (category == null)
            {
                ModelState.AddModelError("Error", "Main category not found");
                return BadRequest(ModelState);
            }

            var model = new ContactSubcategory
            {
                Name = categoryDTO.Name,
                SubcategoryFor = category
            };




            _database.ContactSubcategories.Add(model);
            _database.SaveChanges(); // commit database transaction

            return CreatedAtAction(nameof(GetSubcategories), new { id = categoryDTO.Id }, categoryDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult DeleteSubcategory(int id)
        {
            if (id < 1) // ids start from 1
            {
                return BadRequest();
            }

            // find the contact to delete
            var category = _database.ContactSubcategories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _database.ContactSubcategories.Remove(category);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult UpdateSubcategory(int id, [FromBody] ContactCategoryDTO categoryDTO)
        {
            // check if the request is valid
            if (categoryDTO == null || id != categoryDTO.Id || categoryDTO.Id < 1)
            {
                return BadRequest();
            }

            // find the category to update
            var contact = _database.ContactSubcategories.AsNoTracking().FirstOrDefault(u => u.Id == categoryDTO.Id);
            if (contact == null)
            {
                return NotFound();
            }

            ContactSubcategory model = new()
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name
            };

            _database.ContactSubcategories.Update(model);
            _database.SaveChanges(); // commit database transaction

            return NoContent();
        }
    }
}
