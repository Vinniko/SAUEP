using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult SetPoll(string serial, string ip, double power, double electricityConsumption, DateTime date)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/setPoll?serial={serial}&ip={ip}&power={power}" +
                $"&electricityConsumption={electricityConsumption}&date={date}");
            _guardian.Secure(() => _pollRepository.Set(new PollModel(serial, serial, power, electricityConsumption, date)));
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
        [HttpPut("updatePoll")]
        public IActionResult UpdatePoll(int id, string serial, string ip, double power, double electricityConsumption, DateTime date)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/updatePoll?id={id}&serial={serial}&ip={ip}&power={power}" +
                $"&electricityConsumption={electricityConsumption}&date={date}");
            _guardian.Secure(() => _pollRepository.Update(id, new PollModel(serial, serial, power, electricityConsumption, date)));
            return Ok();
        }

        [Authorize]
        [HttpDelete("deletePoll")]
        public IActionResult DeletePoll(int id)
        {
            _logger.Logg($"C ip адресса: {Request.HttpContext.Connection.RemoteIpAddress} был выполнен запрос: /api/poll/deletePoll?id={id}");
            _guardian.Secure(() => _pollRepository.Remove<PollModel>(id));
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
