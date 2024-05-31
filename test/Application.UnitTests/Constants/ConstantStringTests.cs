using System.Globalization;
using System.Threading;
using Cfo.Cats.Infrastructure.Constants;
using FluentAssertions;
using FluentValidation;
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
        
        ConstantString.Add.Should().Be("Add");
        ConstantString.Actions.Should().Be("Actions");
        ConstantString.Cancel.Should().Be("Cancel");
        ConstantString.Clear.Should().Be("Clear");
        ConstantString.Close.Should().Be("Close");
        ConstantString.Confirm.Should().Be("Confirm");
        ConstantString.Delete.Should().Be("Delete");
        ConstantString.Download.Should().Be("Download");
        ConstantString.Downloading.Should().Be("Downloading...");
        ConstantString.Edit.Should().Be("Edit");
        ConstantString.Export.Should().Be("Export to Excel");
        ConstantString.Import.Should().Be("Import from Excel");
        ConstantString.New.Should().Be("New");
        ConstantString.Next.Should().Be("Next");
        ConstantString.No.Should().Be("No");
        ConstantString.NoAllowed.Should().Be("No Allowed");
        ConstantString.Ok.Should().Be("OK");
        ConstantString.Previous.Should().Be("Previous");
        ConstantString.Refresh.Should().Be("Refresh");
        ConstantString.Reset.Should().Be("Reset");
        ConstantString.Save.Should().Be("Save");
        ConstantString.SaveChanges.Should().Be("Save Changes");
        ConstantString.Search.Should().Be("Search");
        ConstantString.Signin.Should().Be("Sign In");
        ConstantString.SigninWith.Should().Be("Sign in with {0}");
        ConstantString.Upload.Should().Be("Upload");
        ConstantString.Uploading.Should().Be("Uploading...");
        ConstantString.Yes.Should().Be("Yes");
    }

  
    
}