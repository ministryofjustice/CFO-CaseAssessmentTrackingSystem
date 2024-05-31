using System.ComponentModel;

namespace Cfo.Cats.Domain.Common.Enums;

public enum UploadType
{
    [Description(@"ProfilePictures")]
    ProfilePicture,

    [Description(@"Documents")]
    Document
}
