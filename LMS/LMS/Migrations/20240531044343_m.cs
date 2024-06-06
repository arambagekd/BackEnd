﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    AuthorName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.AuthorName);
                });

            migrationBuilder.CreateTable(
                name: "Cupboard",
                columns: table => new
                {
                    cupboardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupboard", x => x.cupboardID);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RemindNotifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemindNotifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UpdateNotifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    books = table.Column<bool>(type: "bit", nullable: false),
                    ebooks = table.Column<bool>(type: "bit", nullable: false),
                    journals = table.Column<bool>(type: "bit", nullable: false),
                    others = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateNotifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_Users_Users_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CupboardId = table.Column<int>(type: "int", nullable: false),
                    ShelfNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationNo);
                    table.ForeignKey(
                        name: "FK_Locations_Cupboard_CupboardId",
                        column: x => x.CupboardId,
                        principalTable: "Cupboard",
                        principalColumn: "cupboardID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ISBN = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Borrowed = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedByID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookLocation = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ISBN);
                    table.ForeignKey(
                        name: "FK_Resources_Author_AuthorName",
                        column: x => x.AuthorName,
                        principalTable: "Author",
                        principalColumn: "AuthorName",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Resources_Locations_BookLocation",
                        column: x => x.BookLocation,
                        principalTable: "Locations",
                        principalColumn: "LocationNo",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Resources_Users_AddedByID",
                        column: x => x.AddedByID,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Requests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReturnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IssuedByID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BorrowerID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_BorrowerID",
                        column: x => x.BorrowerID,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_IssuedByID",
                        column: x => x.IssuedByID,
                        principalTable: "Users",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CupboardId",
                table: "Locations",
                column: "CupboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ResourceId",
                table: "Requests",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BorrowerID",
                table: "Reservations",
                column: "BorrowerID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IssuedByID",
                table: "Reservations",
                column: "IssuedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ResourceId",
                table: "Reservations",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_AddedByID",
                table: "Resources",
                column: "AddedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_AuthorName",
                table: "Resources",
                column: "AuthorName");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_BookLocation",
                table: "Resources",
                column: "BookLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddedById",
                table: "Users",
                column: "AddedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RemindNotifications");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "UpdateNotifications");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cupboard");
        }
    }
}
