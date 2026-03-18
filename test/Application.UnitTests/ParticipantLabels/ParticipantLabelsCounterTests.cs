#nullable enable
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.ParticipantLabels;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;
using NUnit.Framework;
using Shouldly;
using System.Data;

namespace Cfo.Cats.Application.UnitTests.ParticipantLabels;

public class ParticipantLabelsCounterTests
{
    [Test]
    public void Constructor_WithValidFactory_ShouldCreateInstance()
    {
        var sqlFactory = new TestSqlConnectionFactory();

        var counter = new ParticipantLabelsCounter(sqlFactory);

        counter.ShouldNotBeNull();
    }

    private class TestSqlConnectionFactory : ISqlConnectionFactory
    {
        public IDbConnection CreateOpenConnection() => null!;
        public string GetConnectionString() => "";
    }
}
