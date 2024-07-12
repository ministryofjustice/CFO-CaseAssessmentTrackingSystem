using Cfo.Cats.Application.Common.Interfaces;

namespace Cfo.Cats.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
