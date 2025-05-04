using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotinLite.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptedContentToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notes",
                newName: "EncryptedContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedContent",
                table: "Notes",
                newName: "Content");
        }
    }
}
