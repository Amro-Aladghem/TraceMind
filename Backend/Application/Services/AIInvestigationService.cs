using Application.Dtos;
using Domain.Enities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AIInvestigationService
    {
        private const int MaxPageSize = 200;

        private readonly AppDbContext _context;

        public AIInvestigationService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new completed investigation for an application.
        /// </summary>
        public async Task<CreateInvestigationResponseDto> CreateInvestigationAsync(CreateInvestigationRequestDto request)
        {
            var now = DateTime.UtcNow;

            var investigation = new AIInvestigation
            {
                Id = Guid.NewGuid(),
                ApplicationId = request.ApplicationId,
                Title = request.Title,
                Summary = request.Summary,
                RootCause = request.RootCause,
                Recommendations = request.Recommendations,
                Confidence = request.Confidence,
                Status = eInvestigationStatus.Completed,
                ModelName = request.ModelName,
                StartedAt = now,
                CompletedAt = now
            };

            _context.AIInvestigations.Add(investigation);
            await _context.SaveChangesAsync();

            return new CreateInvestigationResponseDto
            {
                Id = investigation.Id,
                Status = investigation.Status,
                StartedAt = investigation.StartedAt
            };
        }

        /// <summary>
        /// Returns the full details of a single investigation.
        /// </summary>
        public async Task<GetInvestigationByIdResponseDto?> GetInvestigationByIdAsync(GetInvestigationByIdRequestDto request)
        {
            return await _context.AIInvestigations
                .AsNoTracking()
                .Where(i => i.Id == request.InvestigationId)
                .Select(i => new GetInvestigationByIdResponseDto
                {
                    Id = i.Id,
                    ApplicationId = i.ApplicationId,
                    Title = i.Title,
                    Summary = i.Summary,
                    RootCause = i.RootCause,
                    Recommendations = i.Recommendations,
                    Confidence = i.Confidence,
                    Status = i.Status,
                    ModelName = i.ModelName,
                    StartedAt = i.StartedAt,
                    CompletedAt = i.CompletedAt
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns the investigation history of an application with filtering and pagination.
        /// </summary>
        public async Task<GetPreviousInvestigationsResponseDto> GetPreviousInvestigationsAsync(GetPreviousInvestigationsRequestDto request)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

            var query = _context.AIInvestigations
                .AsNoTracking()
                .Where(i => i.ApplicationId == request.ApplicationId);

            if (request.From.HasValue)
                query = query.Where(i => i.StartedAt >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(i => i.StartedAt <= request.To.Value);

            if (request.Status.HasValue)
                query = query.Where(i => i.Status == request.Status.Value);

            var totalCount = await query.CountAsync();

            var investigations = await query
                .OrderByDescending(i => i.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new PreviousInvestigationDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Summary = i.Summary,
                    RootCause = i.RootCause,
                    Status = i.Status,
                    StartedAt = i.StartedAt,
                    CompletedAt = i.CompletedAt
                })
                .ToListAsync();

            return new GetPreviousInvestigationsResponseDto
            {
                Investigations = investigations,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Updates the results of an investigation after the agent finishes its analysis.
        /// </summary>
        public async Task<UpdateInvestigationResponseDto> UpdateInvestigationAsync(UpdateInvestigationRequestDto request)
        {
            var investigation = await _context.AIInvestigations
                .FirstOrDefaultAsync(i => i.Id == request.InvestigationId);

            if (investigation is null)
            {
                return new UpdateInvestigationResponseDto
                {
                    Id = request.InvestigationId,
                    Success = false
                };
            }

            investigation.Title = request.Title;
            investigation.Summary = request.Summary;
            investigation.RootCause = request.RootCause;
            investigation.Recommendations = request.Recommendations;
            investigation.ModelName = request.ModelName;

            await _context.SaveChangesAsync();

            return new UpdateInvestigationResponseDto
            {
                Id = investigation.Id,
                Success = true
            };
        }

        /// <summary>
        /// Updates the investigation status while it is running, completed or failed.
        /// </summary>
        public async Task<UpdateInvestigationStatusResponseDto> UpdateInvestigationStatusAsync(UpdateInvestigationStatusRequestDto request)
        {
            var investigation = await _context.AIInvestigations
                .FirstOrDefaultAsync(i => i.Id == request.InvestigationId);

            if (investigation is null)
            {
                return new UpdateInvestigationStatusResponseDto
                {
                    Id = request.InvestigationId,
                    Status = request.Status,
                    Success = false
                };
            }

            investigation.Status = request.Status;

            if (request.Status == eInvestigationStatus.Completed || request.Status == eInvestigationStatus.Failed)
                investigation.CompletedAt ??= DateTime.UtcNow;
            else
                investigation.CompletedAt = null;

            await _context.SaveChangesAsync();

            return new UpdateInvestigationStatusResponseDto
            {
                Id = investigation.Id,
                Status = investigation.Status,
                Success = true
            };
        }
    }
}
