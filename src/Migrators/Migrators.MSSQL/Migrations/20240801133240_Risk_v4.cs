using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class Risk_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RiskToStaff",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToStaffInCustody");

            migrationBuilder.RenameColumn(
                name: "RiskToSelf",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToStaffInCommunity");

            migrationBuilder.RenameColumn(
                name: "RiskToPublic",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToSelfInCustody");

            migrationBuilder.RenameColumn(
                name: "RiskToOtherPrisoners",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToSelfInCommunity");

            migrationBuilder.RenameColumn(
                name: "RiskToKnownAdult",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToPublicInCustody");

            migrationBuilder.RenameColumn(
                name: "RiskToChildren",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToPublicInCommunity");

            migrationBuilder.AddColumn<int>(
                name: "RiskToChildrenInCommunity",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToChildrenInCustody",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToKnownAdultInCommunity",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToKnownAdultInCustody",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToOtherPrisonersInCommunity",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskToOtherPrisonersInCustody",
                schema: "Participant",
                table: "Risk",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiskToChildrenInCommunity",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToChildrenInCustody",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToKnownAdultInCommunity",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToKnownAdultInCustody",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToOtherPrisonersInCommunity",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.DropColumn(
                name: "RiskToOtherPrisonersInCustody",
                schema: "Participant",
                table: "Risk");

            migrationBuilder.RenameColumn(
                name: "RiskToStaffInCustody",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToStaff");

            migrationBuilder.RenameColumn(
                name: "RiskToStaffInCommunity",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToSelf");

            migrationBuilder.RenameColumn(
                name: "RiskToSelfInCustody",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToPublic");

            migrationBuilder.RenameColumn(
                name: "RiskToSelfInCommunity",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToOtherPrisoners");

            migrationBuilder.RenameColumn(
                name: "RiskToPublicInCustody",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToKnownAdult");

            migrationBuilder.RenameColumn(
                name: "RiskToPublicInCommunity",
                schema: "Participant",
                table: "Risk",
                newName: "RiskToChildren");
        }
    }
}
