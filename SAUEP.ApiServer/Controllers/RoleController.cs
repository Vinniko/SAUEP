using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Repositories;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Constructors

        public RoleController(IParser jsonParser, IGuardian guardian, ILogger logger, RoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _guardian = guardian;
            _logger = logger;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Http Methods

        [Authorize]
        [HttpPost("setRole")]
        public IActionResult SetRole(string title)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/role/setRole?title={title}");
            var exception = _guardian.Secure(() => _roleRepository.Set(new RoleModel(title))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpGet("getRoles")]
        public string GetRoles()
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/roles/getRoles");
            return _guardian.Secure(() => _jsonParser.Depars(_roleRepository.Get<RoleModel>())).Value;
        }

        [Authorize]
        [HttpGet("getRole")]
        public string GetRole(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/role/getRole?id={id}");
            return _guardian.Secure(() => _jsonParser.Depars(_roleRepository.GetById(id) as RoleModel)).Value;
        }

        [Authorize]
        [HttpPut("updateRole")]
        public IActionResult UpdateRole(int id, string title)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/role/updateRole?id={id}&title={title}");
            var exception = _guardian.Secure(() => _roleRepository.Update(id, new RoleModel(title, default))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteRole")]
        public IActionResult DeleteRole(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/role/deleteRole?id={id}");
            var exception = _guardian.Secure(() => _roleRepository.Remove<RoleModel>(id)).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        #endregion



        #region Fields

        private IRepository _roleRepository;
        private IGuardian _guardian;
        private ILogger _logger;
        private IParser _jsonParser;

        #endregion
    }
}
