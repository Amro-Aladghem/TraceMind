using Application.Dtos;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Domain.Enities.Application;

namespace Application.Services
{
    public class ApplicationService
    {
        private readonly AppDbContext _context;

        public ApplicationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetApplicationInfoResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request)
        {
            var application = new ApplicationEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Domain = request.Domain,
                Environment = request.Environment,
                IpAddress = request.IpAddress,
                CreationTime = DateTime.UtcNow,
                IsActive = true
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return ToDto(application);
        }

        private static GetApplicationInfoResponseDto ToDto(ApplicationEntity application)
        {
            return new GetApplicationInfoResponseDto
            {
                Id = application.Id,
                Name = application.Name,
                Domain = application.Domain,
                Environment = application.Environment,
                CreatedAt = application.CreationTime,
                IsActive = application.IsActive
            };
        }
    }
}
