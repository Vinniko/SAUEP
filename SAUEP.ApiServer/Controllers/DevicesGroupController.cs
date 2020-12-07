using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Repositories;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesGroupController : ControllerBase
    {
        #region Constructors

        public DevicesGroupController(IParser jsonParser, IGuardian guardian, ILogger logger, DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
            _guardian = guardian;
            _logger = logger;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Http Methods

        [Authorize]
        [HttpPost("setDeviceGroup")]
        public IActionResult SetDeviceGroup(string title)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/devicesgroup/setDeviceGroup?title={title}");
            _guardian.Secure(() => _deviceGroupRepository.Set(new DeviceGroupModel(title)));
            return Ok();
        }

        [Authorize]
        [HttpGet("getDeviceGroups")]
        public string GetDeviceGroups()
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/devicesgroup/getDeviceGroups");
            return _guardian.Secure(() => _jsonParser.Depars(_deviceGroupRepository.Get<DeviceGroupModel>())).Value;
        }

        [Authorize]
        [HttpGet("getDeviceGroup")]
        public string GetDeviceGroup(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/devicesgroup/getDeviceGroup?id={id}");
            return _guardian.Secure(() => _jsonParser.Depars(_deviceGroupRepository.GetById(id) as DeviceGroupModel)).Value;
        }

        [Authorize]
        [HttpPut("updateDeviceGroup")]
        public IActionResult UpdateDeviceGroup(int id, string title)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/devicesgroup/updateDeviceGroup?id={id}&title={title}");
            var exception = _guardian.Secure(() => _deviceGroupRepository.Update(id, new DeviceGroupModel(title))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: (int)BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteDeviceGroup")]
        public IActionResult DeleteDeviceGroup(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/devicesgroup/deleteDeviceGroup?id={id}");
            _guardian.Secure(() => _deviceGroupRepository.Remove<DeviceGroupModel>(id));
            return Ok();
        }

        #endregion



        #region Fields

        private IRepository _deviceGroupRepository;
        private IGuardian _guardian;
        private ILogger _logger;
        private IParser _jsonParser;

        #endregion
    }
}
