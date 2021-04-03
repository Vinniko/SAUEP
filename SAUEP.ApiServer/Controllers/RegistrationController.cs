using Microsoft.AspNetCore.Mvc;
using SAUEP.ApiServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [Produces("application/json")]
    public class RegistrationController : ControllerBase
    {
        #region Constructors

        public RegistrationController(IGuardian guardian,  ILogger logger, IRegistration registration)
        {
            _guardian = guardian;
            _registration = registration;
            _logger = logger;
        }

        #endregion



        #region Http Methods

        [Authorize(Roles = "Администратор")]
        [HttpPost("/reg")]
        public IActionResult Reg(string login, string password, string email)
        {
            _logger.Logg($"Попытка регистрации аккаунта {login} с ip адресса: {Request.HttpContext.Connection.RemoteIpAddress}");
            var userModel = new UserModel(login, password, email);
            if (_guardian.Secure(() => _registration.Registration(userModel)).Value)
            {
                _logger.Logg($"Аккаунт {login} зарегестрирован с ip адресса: {Request.HttpContext.Connection.RemoteIpAddress}");
                return Ok();
            }
            else
            {
                _logger.Logg($"Попытка регистрации существующего {login} с ip адресса: {Request.HttpContext.Connection.RemoteIpAddress}");
                return Problem(detail: "User exsists", statusCode: BadRequest().StatusCode);
            }   
        }

        #endregion



        #region Fields

        IGuardian _guardian;
        IRegistration _registration;
        ILogger _logger;

        #endregion

    }
}
