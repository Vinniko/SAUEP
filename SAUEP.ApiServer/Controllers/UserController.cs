using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Repositories;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Constructors

        public UserController(IParser jsonParser, IGuardian guardian, ILogger logger, UserRepository userRepository)
        {
            _userRepository = userRepository;
            _guardian = guardian;
            _logger = logger;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Http Methods

        [Authorize]
        [HttpGet("getUsers")]
        public string GetUsers()
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/user/getUsers");
            return _guardian.Secure(()=>_jsonParser.Depars(_userRepository.Get<UserModel>())).Value;
        }

        [Authorize]
        [HttpGet("getUser")]
        public string GetUser(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/user/getUser?id={id}");
            return _guardian.Secure(() => _jsonParser.Depars(_userRepository.GetById(id) as UserModel)).Value;
        }

        [Authorize]
        [HttpPut("updateUser")]
        public IActionResult UpdateUser(int id, string login, string password, string email, string role)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/user/updateUser?id={id}&login={login}&password={password}&email={email}&role={role}");
            var exception = _guardian.Secure(() => _userRepository.Update(id, new UserModel(login, password, email, default, role))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: (int)BadRequest().StatusCode);
            return Ok();
        }

        [Authorize(Roles = "Администратор")]
        [HttpDelete("deleteUser")]
        public IActionResult DeleteUser(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/user/deleteUser?id={id}");
            _guardian.Secure(() => _userRepository.Remove<UserModel>(id));
            return Ok();
        }

        #endregion



        #region Fields

        private IRepository _userRepository;
        private IGuardian _guardian;
        private ILogger _logger;
        private IParser _jsonParser;

        #endregion
    }
}
