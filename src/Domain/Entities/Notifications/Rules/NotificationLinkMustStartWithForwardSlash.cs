using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities.Notifications.Rules;

public class NotificationLinkMustStartWithForwardSlash(string link) : IBusinessRule
{
    public string Message => "Notification links must begin with a forward slash";

    public bool IsBroken() => link.StartsWith('/') == false;
    
}