// -----------------------------------------------------------------------
// <copyright file="20240610162757_Add-Updatedby-and-index.cs" company="Jack Henry &amp; Associates, Inc.">
// Copyright (c) Jack Henry &amp; Associates, Inc.
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CssWebApi.CssWebApi.Features.EfSample.EfCore.Migrations
{
    public partial class AddUpdatedbyandindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SampleEntity",
                type: "STRING(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleEntity_InstitutionUniversalId",
                table: "SampleEntity",
                column: "InstitutionUniversalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SampleEntity_InstitutionUniversalId",
                table: "SampleEntity");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SampleEntity");
        }
    }
}
