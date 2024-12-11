using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Payables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityPqaQueueCreatedDomainEvent(ActivityPqaQueueEntry entity) : CreatedDomainEvent<ActivityPqaQueueEntry>(entity)
    {
    }
}
