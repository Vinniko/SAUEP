using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Configs;
using SAUEP.ApiServer.Interfaces;

namespace SAUEP.ApiServer.Controllers
{
    
    [Produces("application/json")]
    public class AuthorizationController : Controller
    {
        #region Constructors

        public AuthorizationController(IGuardian guardian, ILogger logger, IAuthorization authorization, IParser jsonParser)
        {
            _guardian = guardian;
            _authorization = authorization;
            _jsonParser = jsonParser;
            _logger = logger;
        }

        #endregion



        #region HttpMethods

        [HttpPost("/auth")]
        public IActionResult Auth(string username, string password)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /auth");
            var identity = _guardian.Secure(() => _authorization.Authorize(username, password)).Value;
            if (identity == null)
            {
                _logger.Logg($"Попытка входа на аккаунт {username} с ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} не удалась");
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            _logger.Logg($"{username} с ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} успешно прошёл авторизацию");
            return Json(response);
        }

        #endregion



        #region Fields

        IGuardian _guardian;
        IAuthorization _authorization;
        IParser _jsonParser;
        ILogger _logger;

        #endregion
    }
}
