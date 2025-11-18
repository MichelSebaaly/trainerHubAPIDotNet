using Microsoft.AspNetCore.Http;
using ServiceContracts;
using System.Security.Claims;

namespace Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public int? UserId
        {
            get
            {
                string? claim = _contextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return int.TryParse(claim, out int id) ? id : (int?)null;
            }
        }

    }
}
