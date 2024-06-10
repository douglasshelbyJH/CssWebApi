// -----------------------------------------------------------------------
// <copyright file="20240610161312_InitialCreate.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "STRING(36)", maxLength: 36, nullable: false),
                    InstitutionUniversalId = table.Column<string>(type: "STRING(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleEntity");
        }
    }
}
