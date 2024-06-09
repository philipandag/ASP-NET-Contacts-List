using Microsoft.EntityFrameworkCore;
using ASP_NET_Contacts_List.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_Contacts_List.Data
{
    public class ApplicationDbContext : IdentityDbContext<Contact, IdentityRole<int>, int>
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactCategory> ContactCategories { get; set; }
        public DbSet<ContactSubCategory> ContactSubCategories { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var categories = new ContactCategory[]
            {
                new ContactCategory
                {
                    Id = 1,
                    Name = "Work"
                },
                new ContactCategory
                {
                    Id = 2,
                    Name = "Private"
                },
                new ContactCategory
                {
                    Id = 3,
                    Name = "Other"
                },
            };
            modelBuilder.Entity<ContactCategory>().HasData(
                categories
            );

            modelBuilder.Entity<ContactSubCategory>().HasOne(subcategory => subcategory.SubcategoryFor).WithMany(category => category.SubCategories);
            modelBuilder.Entity<ContactSubCategory>().HasData(
                new
                {
                    Id = 1,
                    Name = "Boss",
                    SubcategoryForId = categories.FirstOrDefault(u => u.Name == "Work").Id
                },
                new
                {
                    Id = 2,
                    Name = "Client",
                    SubcategoryForId = categories.FirstOrDefault(u => u.Name == "Work").Id
                },
                new
                {
                    Id = 3,
                    Name = "Family",
                    SubcategoryForId = categories.FirstOrDefault(u => u.Name == "Private").Id
                }
            );

            modelBuilder.Entity<Contact>().HasOne(contact => contact.MainCategory).WithMany(category => category.ContactsWithCategory);
            modelBuilder.Entity<Contact>().HasOne(contact => contact.SubCategory).WithMany(subcategory => subcategory.ContactsWithSubcategory);
            modelBuilder.Entity<Contact>().HasData(
                new
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john.doe@example.com",
                    PasswordHash = new PasswordHasher<Contact>().HashPassword(null, "Haslo1"),     
                    PhoneNumber = "213465743",
                    MainCategoryId = 1,
                    SubCategoryId = 1,
                    DateOfBirth = new System.DateTime(1980, 1, 1),

                    AccessFailedCount = 0,
                    EmailConfirmed = false,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    SecurityStamp = "security_stamp1",
                    ConcurrencyStamp = "concurrency_stamp1",
                    NormalizedEmail = ""
                },
                new
                {
                    Id = 2,
                    Name = "Admin",
                    Surname = "Admin",
                    Email = "admin@admin.com",
                    PasswordHash = new PasswordHasher<Contact>().HashPassword(null, "admin"),
                    PhoneNumber = "123456789",
                    MainCategoryId = 1,
                    SubCategoryId = 1,
                    DateOfBirth = new System.DateTime(2024, 2, 28),
                    AccessFailedCount = 0,
                    EmailConfirmed = false,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    SecurityStamp = "security_stamp2",
                    ConcurrencyStamp = "concurrency_stamp2",
                    NormalizedEmail = ""
                }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
