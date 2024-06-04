using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Application.Common.Security;


public class RequestAuthorizeAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthorizeAttribute" /> class.
    /// </summary>
    public RequestAuthorizeAttribute() { }

    /// <summary>
    ///     Gets or sets a comma delimited list of roles that are allowed to access the resource.
    /// </summary>
    public string Roles { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the policy name that determines access to the resource.
    /// </summary>
    public string Policy { get; set; } = string.Empty;
}