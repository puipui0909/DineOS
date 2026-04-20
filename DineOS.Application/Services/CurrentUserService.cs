using DineOS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DineOS.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetRestaurantId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("restaurantId");

            if (claim == null)
                throw new UnauthorizedAccessException("Không tìm thấy restaurantId");

            return Guid.Parse(claim.Value);
        }
    }
}