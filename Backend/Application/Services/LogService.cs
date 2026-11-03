using Application.Dtos;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class LogService
    {
        private const int MaxLimit = 200;

        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new log entry for an application.
        /// </summary>
        public async Task<LogDto> CreateLog(Guid applicationId, LogDto dto)
        {
            var log = new Domain.Enities.Log
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                ApplicationId = applicationId,
                Timestamp = dto.Timestamp == default ? DateTimeOffset.UtcNow : ToDateTimeOffset(dto.Timestamp),
                Level = dto.Level,
                Message = dto.Message,
                Exception = dto.Exception,
                Properties = dto.Properties,
                RequestPath = dto.RequestPath,
                RequestMethod = dto.RequestMethod,
                StatusCode = dto.StatusCode,
                TraceId = dto.TraceId,
                IpAddress = dto.IpAddress,
                Domain = dto.Domain
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return ToDto(log);
        }

        /// <summary>
        /// Returns logs for an application with optional filters and pagination.
        /// </summary>
        public async Task<GetLogsResponseDto> GetLogsAsync(GetLogsRequestDto request)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 50 : Math.Min(request.PageSize, MaxLimit);

            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId);

            if (request.Level.HasValue)
                query = query.Where(l => l.Level == request.Level.Value);

            if (request.From.HasValue)
            {
                var from = ToDateTimeOffset(request.From.Value);
                query = query.Where(l => l.Timestamp >= from);
            }

            if (request.To.HasValue)
            {
                var to = ToDateTimeOffset(request.To.Value);
                query = query.Where(l => l.Timestamp <= to);
            }

            if (!string.IsNullOrWhiteSpace(request.RequestPath))
            {
                var requestPath = request.RequestPath.Trim();
                query = query.Where(l => l.RequestPath != null && l.RequestPath.Contains(requestPath));
            }

            if (!string.IsNullOrWhiteSpace(request.RequestMethod))
            {
                var requestMethod = request.RequestMethod.Trim().ToUpper();
                query = query.Where(l => l.RequestMethod != null && l.RequestMethod.ToUpper() == requestMethod);
            }

            if (request.StatusCode.HasValue)
                query = query.Where(l => l.StatusCode == request.StatusCode.Value);

            if (!string.IsNullOrWhiteSpace(request.TraceId))
            {
                var traceId = request.TraceId.Trim();
                query = query.Where(l => l.TraceId == traceId);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => ToDto(l))
                .ToListAsync();

            return new GetLogsResponseDto
            {
                Logs = logs,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Returns the most recent Error and Fatal logs for an application.
        /// Warnings are intentionally excluded.
        /// </summary>
        public async Task<GetRecentErrorsResponseDto> GetRecentErrorsAsync(GetRecentErrorsRequestDto request)
        {
            var limit = request.Limit < 1 ? 20 : Math.Min(request.Limit, MaxLimit);

            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId &&
                            (l.Level == eLogLevel.Error || l.Level == eLogLevel.Fatal));

            if (request.From.HasValue)
            {
                var from = ToDateTimeOffset(request.From.Value);
                query = query.Where(l => l.Timestamp >= from);
            }

            var totalCount = await query.CountAsync();

            var errors = await query
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .Select(l => ToDto(l))
                .ToListAsync();

            return new GetRecentErrorsResponseDto
            {
                Errors = errors,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Returns all logs that share the same HTTP request trace id, oldest first.
        /// </summary>
        public async Task<GetLogsByTraceIdResponseDto> GetLogsByTraceIdAsync(GetLogsByTraceIdRequestDto request)
        {
            var traceId = request.TraceId.Trim();

            var logs = await _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId && l.TraceId == traceId)
                .OrderBy(l => l.Timestamp)
                .Select(l => ToDto(l))
                .ToListAsync();

            return new GetLogsByTraceIdResponseDto
            {
                TraceId = traceId,
                Logs = logs
            };
        }

        /// <summary>
        /// Full-text search inside log messages and exception details.
        /// </summary>
        public async Task<SearchLogsResponseDto> SearchLogsAsync(SearchLogsRequestDto request)
        {
            var limit = request.Limit < 1 ? 50 : Math.Min(request.Limit, MaxLimit);
            var searchTerm = request.SearchTerm.Trim().ToLower();

            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId);

            if (request.From.HasValue)
            {
                var from = ToDateTimeOffset(request.From.Value);
                query = query.Where(l => l.Timestamp >= from);
            }

            if (request.To.HasValue)
            {
                var to = ToDateTimeOffset(request.To.Value);
                query = query.Where(l => l.Timestamp <= to);
            }

            if (request.Level.HasValue)
                query = query.Where(l => l.Level == request.Level.Value);

            query = query.Where(l =>
                l.Message.ToLower().Contains(searchTerm) ||
                (l.Exception != null && l.Exception.ToLower().Contains(searchTerm)));

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .Select(l => ToDto(l))
                .ToListAsync();

            return new SearchLogsResponseDto
            {
                Logs = logs,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Counts Error and Fatal logs within a time range.
        /// </summary>
        public async Task<GetErrorCountResponseDto> GetErrorCountAsync(GetErrorCountRequestDto request)
        {
            var from = ToDateTimeOffset(request.From);
            var to = ToDateTimeOffset(request.To);

            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId &&
                            l.Timestamp >= from &&
                            l.Timestamp <= to);

            var errorCount = await query.CountAsync(l => l.Level == eLogLevel.Error);
            var fatalCount = await query.CountAsync(l => l.Level == eLogLevel.Fatal);

            return new GetErrorCountResponseDto
            {
                TotalErrors = errorCount + fatalCount,
                ErrorCount = errorCount,
                FatalCount = fatalCount
            };
        }

        /// <summary>
        /// Groups errors by exception type to surface the most recurring problems.
        /// </summary>
        public async Task<GetMostFrequentErrorsResponseDto> GetMostFrequentErrorsAsync(GetMostFrequentErrorsRequestDto request)
        {
            var limit = request.Limit < 1 ? 10 : Math.Min(request.Limit, MaxLimit);
            var from = ToDateTimeOffset(request.From);
            var to = ToDateTimeOffset(request.To);

            var errors = await _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId &&
                            l.Timestamp >= from &&
                            l.Timestamp <= to &&
                            (l.Level == eLogLevel.Error || l.Level == eLogLevel.Fatal))
                .Select(l => new { l.Exception, l.Message, l.Timestamp })
                .ToListAsync();

            var frequentErrors = errors
                .GroupBy(e => ExtractExceptionType(e.Exception))
                .Select(group => new FrequentErrorDto
                {
                    ExceptionType = group.Key,
                    SampleMessage = ExtractExceptionMessage(group.First().Exception) ?? group.First().Message,
                    Count = group.Count(),
                    LastOccurredAt = group.Max(e => e.Timestamp).UtcDateTime
                })
                .OrderByDescending(e => e.Count)
                .Take(limit)
                .ToList();

            return new GetMostFrequentErrorsResponseDto
            {
                Errors = frequentErrors
            };
        }

        /// <summary>
        /// Returns HTTP requests whose duration exceeds the given threshold.
        /// </summary>
        public async Task<GetSlowRequestsResponseDto> GetSlowRequestsAsync(GetSlowRequestsRequestDto request)
        {
            var limit = request.Limit < 1 ? 20 : Math.Min(request.Limit, MaxLimit);
            var from = ToDateTimeOffset(request.From);
            var to = ToDateTimeOffset(request.To);

            var requests = await _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId &&
                            l.Timestamp >= from &&
                            l.Timestamp <= to &&
                            l.RequestPath != null &&
                            l.DurationMs != null &&
                            l.DurationMs >= request.MinDurationMs)
                .OrderByDescending(l => l.DurationMs)
                .Take(limit)
                .Select(l => new
                {
                    l.RequestPath,
                    l.RequestMethod,
                    l.StatusCode,
                    l.DurationMs,
                    l.Timestamp,
                    l.TraceId
                })
                .ToListAsync();

            return new GetSlowRequestsResponseDto
            {
                Requests = requests
                    .Select(l => new SlowRequestDto
                    {
                        RequestPath = l.RequestPath!,
                        RequestMethod = l.RequestMethod ?? string.Empty,
                        StatusCode = l.StatusCode,
                        DurationMs = l.DurationMs!.Value,
                        Timestamp = l.Timestamp.UtcDateTime,
                        TraceId = l.TraceId
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Aggregated HTTP request statistics. A request is considered failed when
        /// its status code is greater than or equal to 400.
        /// </summary>
        public async Task<GetRequestStatisticsResponseDto> GetRequestStatisticsAsync(GetRequestStatisticsRequestDto request)
        {
            var from = ToDateTimeOffset(request.From);
            var to = ToDateTimeOffset(request.To);

            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ApplicationId == request.ApplicationId &&
                            l.Timestamp >= from &&
                            l.Timestamp <= to &&
                            l.RequestPath != null);

            var totalRequests = await query.CountAsync();
            var successfulRequests = await query.CountAsync(l => l.StatusCode != null && l.StatusCode < 400);
            var failedRequests = await query.CountAsync(l => l.StatusCode != null && l.StatusCode >= 400);

            var averageDurationMs = await query
                .Where(l => l.DurationMs != null)
                .Select(l => l.DurationMs)
                .AverageAsync() ?? 0;

            var maxDurationMs = await query
                .Where(l => l.DurationMs != null)
                .MaxAsync(l => l.DurationMs) ?? 0;

            var errorRate = totalRequests == 0
                ? 0
                : Math.Round(failedRequests * 100.0 / totalRequests, 2);

            return new GetRequestStatisticsResponseDto
            {
                TotalRequests = totalRequests,
                SuccessfulRequests = successfulRequests,
                FailedRequests = failedRequests,
                AverageDurationMs = Math.Round(averageDurationMs, 2),
                MaxDurationMs = maxDurationMs,
                ErrorRate = errorRate
            };
        }

        /// <summary>
        /// Returns basic information about the application under investigation.
        /// </summary>
        public async Task<GetApplicationInfoResponseDto?> GetApplicationInfoAsync(GetApplicationInfoRequestDto request)
        {
            return await _context.Applications
                .AsNoTracking()
                .Where(a => a.Id == request.ApplicationId)
                .Select(a => new GetApplicationInfoResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Domain = a.Domain,
                    Environment = a.Environment,
                    CreatedAt = a.CreationTime,
                    IsActive = a.IsActive
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns today's Error and Fatal logs grouped by request path and message.
        /// </summary>
        public async Task<List<DailyErrorLogDto>> GetDailyErrorLogsAsync(Guid applicationId)
        {
            var today = ToDateTimeOffset(DateTime.Today);
            var tomorrow = ToDateTimeOffset(DateTime.Today.AddDays(1));

            return await _context.Logs
                .AsNoTracking()
                .Where(l =>
                    l.ApplicationId == applicationId &&
                    l.Timestamp >= today &&
                    l.Timestamp < tomorrow &&
                    (l.Level == eLogLevel.Error ||
                     l.Level == eLogLevel.Fatal))
                .GroupBy(l => new
                {
                    l.RequestPath,
                    l.Message
                })
                .Select(g => new DailyErrorLogDto
                {
                    RequestPath = g.Key.RequestPath,
                    Message = g.Key.Message,
                    Count = g.LongCount()
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();
        }

        private static LogDto ToDto(Domain.Enities.Log l)
        {
            return new LogDto
            {
                Id = l.Id,
                Timestamp = l.Timestamp.UtcDateTime,
                Level = l.Level,
                Message = l.Message,
                Exception = l.Exception,
                Properties = l.Properties,
                RequestPath = l.RequestPath,
                RequestMethod = l.RequestMethod,
                StatusCode = l.StatusCode,
                TraceId = l.TraceId,
                IpAddress = l.IpAddress,
                Domain = l.Domain
            };
        }

        private static DateTimeOffset ToDateTimeOffset(DateTime value)
        {
            return value.Kind == DateTimeKind.Unspecified
                ? new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc))
                : new DateTimeOffset(value.ToUniversalTime());
        }

        private static string ExtractExceptionType(string? exception)
        {
            if (string.IsNullOrWhiteSpace(exception))
                return "Unknown";

            var firstLine = exception.Split('\n')[0].Trim();
            var typePart = firstLine.Split(':')[0].Trim();
            var lastDot = typePart.LastIndexOf('.');

            var type = lastDot >= 0 ? typePart[(lastDot + 1)..] : typePart;

            return string.IsNullOrWhiteSpace(type) ? "Unknown" : type;
        }

        private static string? ExtractExceptionMessage(string? exception)
        {
            if (string.IsNullOrWhiteSpace(exception))
                return null;

            var firstLine = exception.Split('\n')[0].Trim();
            var separatorIndex = firstLine.IndexOf(':');

            if (separatorIndex < 0 || separatorIndex + 1 >= firstLine.Length)
                return firstLine;

            return firstLine[(separatorIndex + 1)..].Trim();
        }
    }
}
