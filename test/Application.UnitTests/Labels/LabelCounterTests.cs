#nullable enable
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Labels;
using Cfo.Cats.Domain.Labels;
using Dapper;
using Moq;
using Moq.Dapper;
using NUnit.Framework;
using Shouldly;
using System.Data;

namespace Cfo.Cats.Application.UnitTests.Labels;

public class LabelCounterTests
{
    private static readonly TestCaseData[] ExpectedCounts =
    [
        new TestCaseData(0).SetName("CountParticipants_WhenNoParticipants_ShouldReturn0"),
        new TestCaseData(1).SetName("CountParticipants_WhenOneParticipants_ShouldReturn1"),
        new TestCaseData(10).SetName("CountParticipants_WhenTenParticipants_ShouldReturn10"),
    ];

    [TestCaseSource(nameof(ExpectedCounts))]
    public void CountParticipants_ShouldReturnExpectedCount(int expectedCount)
    {
        var connection = new Mock<IDbConnection>();
        connection
            .SetupDapper(c => c.QuerySingle<int>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .Returns(expectedCount);

        var sqlFactory = new Mock<ISqlConnectionFactory>();
        sqlFactory.Setup(x => x.CreateOpenConnection()).Returns(connection.Object);

        var counter = new LabelCounter(sqlFactory.Object);
        var labelId = new LabelId(Guid.NewGuid());

        var count = counter.CountParticipants(labelId);

        count.ShouldBe(expectedCount);
    }
}
