using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_NET_Contacts_List.Migrations
{
    /// <inheritdoc />
    public partial class AddContactsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Category", "DateOfBirth", "Email", "Name", "Password", "PhoneNumber", "SubCategory" },
                values: new object[,]
                {
                    { 1, "Friend", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John Doe", "password_hash1", "213465743", "Close" },
                    { 2, "Friend", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.doe@example.com", "Jane Doe", "password_hash2", "123456789", "Close" },
                    { 3, "Family", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alice@example.com", "Alice", "password_hash3", "123123123", "Mother" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
