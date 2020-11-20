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

        public RegistrationController(IGuardian guardian,  ILogger logger, IRegistration registration, IParser jsonParser)
        {
            _guardian = guardian;
            _registration = registration;
            _jsonParser = jsonParser;
            _logger = logger;
        }

        #endregion



        #region Http Methods

        [Authorize(Roles = "Администратор")]
        [HttpPost("/reg")]
        public IActionResult Reg(string login, string password, string email, string role = "Пользователь")
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
                return BadRequest("User exsists");
            }
                
        }

        #endregion



        #region Fields

        IGuardian _guardian;
        IRegistration _registration;
        IParser _jsonParser;
        ILogger _logger;

        #endregion

    }
}
