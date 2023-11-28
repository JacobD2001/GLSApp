using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLSApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RName1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RName3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RZipcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    References = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Pfc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consigns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consigns");
        }
    }
}
