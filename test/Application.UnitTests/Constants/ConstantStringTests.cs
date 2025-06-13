using System.Globalization;
using System.Threading;
using Cfo.Cats.Infrastructure.Constants;
using Shouldly;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.Constants;

public class ConstantStringTests
{
    [Test]
    public void Test()
    {
        // Set the culture to a specific culture, e.g., "en-US"
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-gb");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-gb");

        ConstantString.Add.ShouldBe("Add");
        ConstantString.Actions.ShouldBe("Actions");
        ConstantString.Cancel.ShouldBe("Cancel");
        ConstantString.Clear.ShouldBe("Clear");
        ConstantString.Close.ShouldBe("Close");
        ConstantString.Confirm.ShouldBe("Confirm");
        ConstantString.Delete.ShouldBe("Delete");
        ConstantString.Download.ShouldBe("Download");
        ConstantString.Downloading.ShouldBe("Downloading...");
        ConstantString.Edit.ShouldBe("Edit");
        ConstantString.Export.ShouldBe("Export");
        ConstantString.Import.ShouldBe("Import from Excel");
        ConstantString.New.ShouldBe("New");
        ConstantString.Next.ShouldBe("Next");
        ConstantString.No.ShouldBe("No");
        ConstantString.NoAllowed.ShouldBe("No Allowed");
        ConstantString.Ok.ShouldBe("OK");
        ConstantString.Previous.ShouldBe("Previous");
        ConstantString.Refresh.ShouldBe("Refresh");
        ConstantString.Reset.ShouldBe("Reset");
        ConstantString.Save.ShouldBe("Save");
        ConstantString.SaveChanges.ShouldBe("Save Changes");
        ConstantString.Search.ShouldBe("Search");
        ConstantString.Signin.ShouldBe("Sign In");
        ConstantString.SigninWith.ShouldBe("Sign in with {0}");
        ConstantString.Upload.ShouldBe("Upload");
        ConstantString.Uploading.ShouldBe("Uploading...");
        ConstantString.Yes.ShouldBe("Yes"); ConstantString.Add.ShouldBe("Add");
        ConstantString.Actions.ShouldBe("Actions");
        ConstantString.Cancel.ShouldBe("Cancel");
        ConstantString.Clear.ShouldBe("Clear");
        ConstantString.Close.ShouldBe("Close");
        ConstantString.Confirm.ShouldBe("Confirm");
        ConstantString.Delete.ShouldBe("Delete");
        ConstantString.Download.ShouldBe("Download");
        ConstantString.Downloading.ShouldBe("Downloading...");
        ConstantString.Edit.ShouldBe("Edit");
        ConstantString.Export.ShouldBe("Export");
        ConstantString.Import.ShouldBe("Import from Excel");
        ConstantString.New.ShouldBe("New");
        ConstantString.Next.ShouldBe("Next");
        ConstantString.No.ShouldBe("No");
        ConstantString.NoAllowed.ShouldBe("No Allowed");
        ConstantString.Ok.ShouldBe("OK");
        ConstantString.Previous.ShouldBe("Previous");
        ConstantString.Refresh.ShouldBe("Refresh");
        ConstantString.Reset.ShouldBe("Reset");
        ConstantString.Save.ShouldBe("Save");
        ConstantString.SaveChanges.ShouldBe("Save Changes");
        ConstantString.Search.ShouldBe("Search");
        ConstantString.Signin.ShouldBe("Sign In");
        ConstantString.SigninWith.ShouldBe("Sign in with {0}");
        ConstantString.Upload.ShouldBe("Upload");
        ConstantString.Uploading.ShouldBe("Uploading...");
        ConstantString.Yes.ShouldBe("Yes");
    }
}