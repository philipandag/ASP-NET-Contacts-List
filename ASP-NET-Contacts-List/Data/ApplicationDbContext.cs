using Microsoft.EntityFrameworkCore;
using ASP_NET_Contacts_List.Models;

namespace ASP_NET_Contacts_List.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasData(
            new Contact
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password_hash1",
                Category = "Friend",
                SubCategory = "Close",
                PhoneNumber = "213465743",

            },
            new Contact
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane.doe@example.com",
                Password = "password_hash2",
                Category = "Friend",
                SubCategory = "Close",
                PhoneNumber = "123456789",
            },
            new Contact
            {
                Id = 3,
                Name = "Alice",
                Email = "alice@example.com",
                Password = "password_hash3",
                Category = "Family",
                SubCategory = "Mother",
                PhoneNumber = "123123123",
                DateOfBirth = new DateTime(1970, 1, 1),
            }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
