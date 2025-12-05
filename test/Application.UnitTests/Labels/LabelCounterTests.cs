#nullable enable
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Labels;
using Cfo.Cats.Domain.Labels;
using NUnit.Framework;
using Shouldly;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Cfo.Cats.Application.UnitTests.Labels;

public class LabelCounterTests
{
    [Test]
    public void CountParticipants_ShouldReturnZero()
    {
        var sqlFactory = new TestSqlConnectionFactory();
        var logger = new TestLogger();
        var counter = new LabelCounter(sqlFactory, logger);
        var labelId = new LabelId(Guid.NewGuid());

        var count = counter.CountParticipants(labelId);

        count.ShouldBe(0);
    }

    [Test]
    public void CountParticipants_ShouldLogWarning()
    {
        var sqlFactory = new TestSqlConnectionFactory();
        var logger = new TestLogger();
        var counter = new LabelCounter(sqlFactory, logger);
        var labelId = new LabelId(Guid.NewGuid());

        counter.CountParticipants(labelId);

        logger.WarningLogged.ShouldBeTrue();
        logger.LogMessage.ShouldContain("Counting participants");
        logger.LogMessage.ShouldContain("is currently not implemented");
    }

    private class TestSqlConnectionFactory : ISqlConnectionFactory
    {
        public IDbConnection GetOpenConnection() => null!;
        public IDbConnection CreateNewConnection() => null!;
        public string GetConnectionString() => "";
    }

    private class TestLogger : ILogger<LabelCounter>
    {
        public bool WarningLogged { get; private set; }
        public string LogMessage { get; private set; } = "";

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception? exception, 
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
            {
                WarningLogged = true;
                LogMessage = formatter(state, exception);
            }
        }
    }
}
