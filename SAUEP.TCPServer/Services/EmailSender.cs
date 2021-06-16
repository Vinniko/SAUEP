using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using SAUEP.TCPServer.Repositories;
using System.Net;
using System.Net.Mail;

namespace SAUEP.TCPServer.Services
{
    public sealed class EmailSender : ISender, IObserver
    {
        #region Constructors

        public EmailSender(DeviceRepository deviceRepository, UserRepository userRepository, IWriter consoleWriter, ILogger logger)
        {
            _deviceRepository = deviceRepository;
            _userRepository = userRepository;
            _consoleWriter = consoleWriter as ConsoleWriter;
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public void Send<T>(T data)
        {
            var users = _userRepository.Get<UserModel>().ToArray();
            for (var i = 0; i < users.Length; i++)
            {
                if(users[i].Role.Equals("Администратор") || users[i].Role.Equals("Модератор"))
                {
                    MailAddress mail = new MailAddress(_senderEmail, "SAUEP Technologi");
                    //MailAddress to = new MailAddress(users[i].Email);
                    MailAddress sendTo = new MailAddress("vinnik_21@bk.ru");
                    MailMessage sendFrom = new MailMessage(mail, sendTo);
                    sendFrom.Subject = "АВАРИЙНЫЙ СЛУЧАЙ";
                    sendFrom.Body = data.ToString(); 
                    SmtpClient smtp = new SmtpClient(_smtpServer, _smtpServerPort);
                    smtp.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(sendFrom);
                    _consoleWriter.Write($"Письмо отправлено на почту: {users[i].Email}");
                    _logger.Logg($"Письмо отправлено на почту: {users[i].Email}");
                }
            }
        }
        public void Update(PollModel pollModel)
        {
            var device = _deviceRepository.Get<DeviceModel>().Where(model => model.Serial.Equals(pollModel.Serial)).Last();
            if (pollModel.Power < device.MinPower && device.Status) 
            {
                Send(_minMessage + $"Проблема с устройством {pollModel.Serial}. Мощность ниже минимальной! Мощность рекомендуемая: {device.MinPower}. Мощность потребляемая: {pollModel.Power}");
            }
            else if (pollModel.Power > device.MaxPower && device.Status)
            {
                Send(_maxMessage + $"Проблема с устройством {pollModel.Serial}. Мощность выше максимальной! Мощность рекомендуемая: {device.MaxPower}. Мощность потребляемая: {pollModel.Power}");
            }
        }

        #endregion



        #region Fields

        private DeviceRepository _deviceRepository;
        private UserRepository _userRepository;
        private ConsoleWriter _consoleWriter;
        private ILogger _logger;
        private const string _minMessage = "Мощность устройства слишком мала!";
        private const string _maxMessage = "Мощность устройства слишком велика!";
        private const string _senderEmail = "sauep228@gmail.com";
        private const string _senderPassword = "******";
        private const string _smtpServer = "smtp.gmail.com";
        private const int _smtpServerPort = 587;


        #endregion
    }
}
