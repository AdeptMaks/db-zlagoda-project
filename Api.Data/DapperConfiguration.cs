using System.Data;
using Dapper;

namespace Api.Data;

public static class DapperConfiguration
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured) return;
        _configured = true;

        SqlMapper.AddTypeHandler(new DateOnlyToDateTimeHandler());
        SqlMapper.AddTypeHandler(new NullableDateOnlyToDateTimeHandler());
    }

    private sealed class DateOnlyToDateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override DateTime Parse(object value) => value switch
        {
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            DateTime dt => dt,
            _ => Convert.ToDateTime(value),
        };

        public override void SetValue(IDbDataParameter parameter, DateTime value)
            => parameter.Value = value;
    }

    private sealed class NullableDateOnlyToDateTimeHandler : SqlMapper.TypeHandler<DateTime?>
    {
        public override DateTime? Parse(object value) => value switch
        {
            null => null,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            DateTime dt => dt,
            _ => Convert.ToDateTime(value),
        };

        public override void SetValue(IDbDataParameter parameter, DateTime? value)
            => parameter.Value = (object?)value ?? DBNull.Value;
    }
}
