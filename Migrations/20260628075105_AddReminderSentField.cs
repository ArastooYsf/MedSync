using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedSync.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderSentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Notifications",
                newName: "RelatedAppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_AppointmentId",
                table: "Notifications",
                newName: "IX_Notifications_RelatedAppointmentId");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Appointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Appointments_RelatedAppointmentId",
                table: "Notifications",
                column: "RelatedAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Appointments_RelatedAppointmentId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "RelatedAppointmentId",
                table: "Notifications",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RelatedAppointmentId",
                table: "Notifications",
                newName: "IX_Notifications_AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
