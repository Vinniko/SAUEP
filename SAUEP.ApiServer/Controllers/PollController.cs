using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SAUEP.ApiServer.Repositories;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : Controller
    {
        #region Constructors

        public PollController(IParser jsonParser, IGuardian guardian, ILogger logger, PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
            _guardian = guardian;
            _logger = logger;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Http Methods

        [Authorize]
        [HttpPost("setPoll")]
        public IActionResult SetPoll(string serial, string ip, double power, double electricityConsumption, string date)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/setPoll?serial={serial}&ip={ip}&power={power}" +
                $"&electricityConsumption={electricityConsumption}&date={date}");
            var exception = _guardian.Secure(() => _pollRepository.Set(new PollModel(serial, serial, power, electricityConsumption, DateTime.Parse(date)))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpGet("getPolls")]
        public string GetPolls()
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/getPolls");
            return _guardian.Secure(() => _jsonParser.Depars(_pollRepository.Get<PollModel>())).Value;
        }

        [Authorize]
        [HttpGet("getPoll")]
        public string GetPoll(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/getPoll?id={id}");
            return _guardian.Secure(() => _jsonParser.Depars(_pollRepository.GetById(id) as PollModel)).Value;
        }

        [Authorize]
        [HttpGet("getLastPolls")]
        public string GetLastPolls(int qty, DateTime dateTime, string serial)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/getLastPolls?qty={qty}&dateTime={dateTime}&serial={serial}");
            return _guardian.Secure(() => _jsonParser.Depars((_pollRepository as PollRepository).Get<PollModel>(qty, dateTime, serial))).Value;
        }

        [Authorize]
        [HttpPut("updatePoll")]
        public IActionResult UpdatePoll(int id, string serial, string ip, double power, double electricityConsumption, DateTime date)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/updatePoll?id={id}&serial={serial}&ip={ip}&power={power}" +
                $"&electricityConsumption={electricityConsumption}&date={date}");
            var exception = _guardian.Secure(() => _pollRepository.Update(id, new PollModel(serial, serial, power, electricityConsumption, date))).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deletePoll")]
        public IActionResult DeletePoll(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/deletePoll?id={id}");
            var exception = _guardian.Secure(() => _pollRepository.Remove<PollModel>(id)).Exception;
            if (exception != null)
                return Problem(detail: exception.Message, statusCode: BadRequest().StatusCode);
            return Ok();
        }

        #endregion



        #region Fields

        private IRepository _pollRepository;
        private IGuardian _guardian;
        private ILogger _logger;
        private IParser _jsonParser;

        #endregion
    }
}
