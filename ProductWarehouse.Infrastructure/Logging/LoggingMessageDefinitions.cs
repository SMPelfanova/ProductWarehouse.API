﻿using Microsoft.Extensions.Logging;

namespace ProductWarehouse.Infrastructure.Logging;

public static partial class LoggingMessageDefinitions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "")]
    public static partial void LogInformationMessage(this ILogger logger, string request);
}