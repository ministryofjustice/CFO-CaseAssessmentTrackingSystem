using System.Globalization;
using System.Resources;

namespace Cfo.Cats.Infrastructure.Constants;

public static class ConstantString
{
    private const string ConstantStringResourceId =
        "Cfo.Cats.Infrastructure.Resources.ConstantString";
    private static readonly ResourceManager rm;

    static ConstantString()
    {
        rm = new ResourceManager(ConstantStringResourceId, typeof(ConstantString).Assembly);
    }

    public static string Localize(string key)
    {
        return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }

    public static string Rename => Localize("Rename");
    public static string Refresh => Localize("Refresh");
    public static string Edit => Localize("Edit");
    public static string View => Localize("View");
    public static string Submit => Localize("Submit");
    public static string Delete => Localize("Delete");
    public static string Archive => Localize("Archive");
    public static string Add => Localize("Add");
    public static string Clone => Localize("Clone");
    public static string New => Localize("New");
    public static string Export => Localize("Export to Excel");
    public static string ExportPDF => Localize("Export to PDF");
    public static string Import => Localize("Import from Excel");
    public static string Actions => Localize("Actions");
    public static string Save => Localize("Save");
    public static string SaveAndNew => Localize("Save & New");
    public static string SaveChanges => Localize("Save Changes");
    public static string Saving => Localize("Saving");
    public static string Cancel => Localize("Cancel");
    public static string Close => Localize("Close");
    public static string Search => Localize("Search");
    public static string Clear => Localize("Clear");
    public static string Reset => Localize("Reset");
    public static string Ok => Localize("OK");
    public static string Confirm => Localize("Confirm");
    public static string Continue => Localize("Continue");
    public static string Complete => Localize("Complete");
    public static string Yes => Localize("Yes");
    public static string No => Localize("No");
    public static string Next => Localize("Next");
    public static string Previous => Localize("Previous");
    public static string Upload => Localize("Upload");
    public static string Download => Localize("Download");
    public static string Uploading => Localize("Uploading...");
    public static string Downloading => Localize("Downloading...");
    public static string NoAllowed => Localize("No Allowed");
    public static string SigninWith => Localize("Sign in with {0}");
    public static string Logout => Localize("Logout");
    public static string Signin => Localize("Sign In");

    public static string SaveSuccess => Localize("Save successfully");
    public static string DeleteSuccess => Localize("Delete successfully");
    public static string DeleteFail => Localize("Delete fail");

    public static string ArchiveSuccess => Localize("Archive successfully");
    public static string ArchiveFail => Localize("Archive fail");
    public static string UpdateSuccess => Localize("AddOrUpdate successfully");
    public static string CreateSuccess => Localize("Create successfully");
    public static string LoginSuccess => Localize("Login successfully");
    public static string LogoutSuccess => Localize("Logout successfully");
    public static string LoginFail => Localize("Login fail");
    public static string LogoutFail => Localize("Logout fail");
    public static string ImportSuccess => Localize("Import successfully");
    public static string ImportFail => Localize("Import fail");
    public static string ExportSuccess => Localize("Export successfully");
    public static string ExportFail => Localize("Export fail");
    public static string UploadSuccess => Localize("Upload successfully");

    public static string Selected => Localize("Selected");
    public static string SelectedTotal => Localize("Selected Total");
    public static string AdvancedSearch => Localize("Advanced Search");
    public static string OrderBy => Localize("Order By");
    public static string CreateAnItem => Localize("Create a new {0}");
    public static string EditTheItem => Localize("Edit the {0}");
    public static string DeleteTheItem => Localize("Delete the {0}");
    public static string DeleteItems => Localize("Delete selected items: {0}");
    public static string DeleteConfirmation =>
        Localize("Are you sure you want to delete this item: {0}?");

    public static string DeleteConfirmationWithId =>
        Localize("Are you sure you want to delete this item with Id: {0}?");

    public static string DeleteConfirmWithSelected =>
        Localize("Are you sure you want to delete the selected items: {0}?");

    public static string ArchiveTheItem => Localize("Archive the {0}");
    public static string ArchiveItems => Localize("Archive selected items: {0}");
    public static string ArchiveConfirmation =>
        Localize("Are you sure you want to archive this item: {0}?");

    public static string ArchiveConfirmationWithId =>
        Localize("Are you sure you want to archive this item with Id: {0}?");

    public static string ArchiveConfirmWithSelected =>
        Localize("Are you sure you want to archive the selected items: {0}?");

    public static string NoRecords => Localize("There are no records to view.");
    public static string Loading => Localize("Loading...");
    public static string Waiting => Localize("Wating...");
    public static string Processing => Localize("Processing...");
    public static string DeleteConfirmationTitle => Localize("Delete Confirmation");
    public static string ArchiveConfirmationTitle => Localize("Archive Confirmation");
    public static string LogoutConfirmationTitle => Localize("Logout Confirmation");

    public static string NewEnrolment => Localize("New Enrolment");
    public static string ResumeEnrolment => Localize("Resume Enrolment");
    
    public static string LogoutConfirmation =>
        Localize("You are attempting to log out of application. Do you really want to log out?");
    public static string AddRightToWork =>
    Localize("Add Right To Work");

    public static string ChangeEnrolmentLocation =>
        Localize("Change enrolment location");

    public static string RightToWork =>
    Localize("Right To Work");
    public static string RightToWorkIsRequiredMessage =>
    Localize("No active Right To Work documentation found for the participant, it is a requirement for non-British/Irish participants.");
    public static string AddConsent =>
    Localize("Add Consent");

    public static string Reassign =>
    Localize("Reassign");
    public static string MarkAsRead =>
    Localize("Mark As Read");
    public static string MarkAsUnread =>
    Localize("Mark As Unread");
    public static string ShowReadNotification =>
    Localize("Show Read Notification");
    public static string ShowUnreadNotification =>
    Localize("Show Unread Notification");

    public static string AddActualReleaseDate =>
    Localize("Add Actual Release Date");
    public static string CompletePRI =>
    Localize("Complete PRI");
    public static string AbandonPRI =>
    Localize("Abandon PRI");
    public static string ViewParticipant => Localize("View Participant");
    public static string PriNoActualReleaseDateWarning => Localize("No Actual Release date provided.");
    public static string PriTTGDueWarningToolTip => Localize("Through the Gate (TTG) is due {0}");
}