using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenericTagHelperExample.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CheckBox = table.Column<bool>(type: "bit", nullable: false),
                    DateTimeTextBox = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailTextBox = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelSelectList = table.Column<int>(type: "int", nullable: false),
                    PasswordTextBox = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectRadio = table.Column<int>(type: "int", nullable: false),
                    TextBox = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upload = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RadioBox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FormModelId = table.Column<int>(type: "int", nullable: true),
                    FormModelId1 = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadioBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadioBox_FormModels_FormModelId",
                        column: x => x.FormModelId,
                        principalTable: "FormModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadioBox_FormModels_FormModelId1",
                        column: x => x.FormModelId1,
                        principalTable: "FormModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadioBox_FormModelId",
                table: "RadioBox",
                column: "FormModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RadioBox_FormModelId1",
                table: "RadioBox",
                column: "FormModelId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "RadioBox");

            migrationBuilder.DropTable(
                name: "FormModels");
        }
    }
}
