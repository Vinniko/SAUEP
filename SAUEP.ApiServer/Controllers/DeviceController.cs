using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Repositories;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        #region Constructors

        public DeviceController(IParser jsonParser, IGuardian guardian, ILogger logger, DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
            _guardian = guardian;
            _logger = logger;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Http Methods

        [Authorize]
        [HttpPost("setDevice")]
        public IActionResult SetDevice(string deviceGroup, string serial, string title, 
            string ip, string port, bool status, 
            double maxPower, double minPower)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/device/setDevice?deviceGroup={deviceGroup}&serial={serial}&title={title}" +
                $"&ip={ip}&port={port}&status={status}&maxPower={maxPower}&minPower={minPower}");
            var exception = _guardian.Secure(() => _deviceRepository.Set(new DeviceModel(deviceGroup, serial, title, ip, port, status, maxPower, minPower))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpGet("getDevices")]
        public string GetDevices()
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/device/getDevices");
            return _guardian.Secure(() => _jsonParser.Depars(_deviceRepository.Get<DeviceModel>())).Value;
        }

        [Authorize]
        [HttpGet("getDevice")]
        public string GetDevice(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/device/getDevice?id={id}");
            return _guardian.Secure(() => _jsonParser.Depars(_deviceRepository.GetById(id) as DeviceModel)).Value;
        }

        [Authorize]
        [HttpPut("updateDevice")]
        public IActionResult UpdateDevice(int id, string deviceGroup, string serial, 
            string title, string ip, string port, 
            bool status, double maxPower, double minPower)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/device/updateDevice?id={id}&deviceGroup={deviceGroup}&serial={serial}&title={title}" +
                $"&ip={ip}&port={port}&status={status}&maxPower={maxPower}&minPower={minPower}");
            var exception  = _guardian.Secure(() => _deviceRepository.Update(id, new DeviceModel(deviceGroup, serial, title, ip, port, status, maxPower, minPower))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteDevice")]
        public IActionResult DeleteDevice(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/device/deleteDevice?id={id}");
            var exception = _guardian.Secure(() => _deviceRepository.Remove<DeviceModel>(id)).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        #endregion



        #region Fields

        private IRepository _deviceRepository;
        private IGuardian _guardian;
        private ILogger _logger;
        private IParser _jsonParser;

        #endregion
    }
}
