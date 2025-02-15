using DLW.BFF.Template.BFF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DLW.BFF.Template.BFF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok(CreateSessionUser(User));

        #region Private Methods
        private static SessionUser CreateSessionUser(ClaimsPrincipal claimsPrincipal)
        {
            return new(
                claimsPrincipal.Identity?.Name,
                claimsPrincipal.Identity?.IsAuthenticated ?? false
            );
        }
        #endregion
    }
}
