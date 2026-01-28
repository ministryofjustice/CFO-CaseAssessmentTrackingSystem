using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ArchivedCases_Add_columns_FirstLastName_and_To : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "Mi",
                table: "ArchivedCase",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "To",
                schema: "Mi",
                table: "ArchivedCase",
                type: "datetime2",
                nullable: true);
            
            migrationBuilder.Sql("""
                                     UPDATE ac
                                     SET FirstName = p.FirstName,
                                         LastName = p.LastName,
                                         [To] = eh.[To],
                                         LocationType =
                                             CASE ac.LocationType
                                                 WHEN '0' THEN 'Wing'
                                                 WHEN '1' THEN 'Feeder'
                                                 WHEN '2' THEN 'Outlying'
                                                 WHEN '3' THEN 'Female'
                                                 WHEN '4' THEN 'Community'
                                                 WHEN '5' THEN 'Hub'
                                                 WHEN '6' THEN 'Satellite'
                                                 WHEN '7' THEN 'Unknown'
                                                 WHEN '8' THEN 'Unmapped Custody'
                                                 WHEN '9' THEN 'Unmapped Community'
                                                 ELSE 'Unknown'
                                             END
                                     FROM Mi.ArchivedCase ac
                                     JOIN Participant.Participant p 
                                         ON ac.ParticipantId = p.Id
                                     JOIN Participant.EnrolmentHistory eh 
                                         ON ac.EnrolmentHistoryId = eh.Id
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Mi",
                table: "ArchivedCase");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "Mi",
                table: "ArchivedCase");

            migrationBuilder.DropColumn(
                name: "To",
                schema: "Mi",
                table: "ArchivedCase");
        }
    }
}
