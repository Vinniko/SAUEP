using System;
using SAUEP.TCPServer.Interfaces;

namespace SAUEP.TCPServer.Models
{
    public sealed class PollModel : IModel
    {
        #region Constructors

        public PollModel() { }
        public PollModel(int id, string serial, string ip, double power, double electricityConsumption, DateTime date)
        {
            Id = id;
            Serial = serial;
            Ip = ip;
            Power = power;
            ElectricityConsumption = electricityConsumption;
            Date = date;
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string Serial { get; set; }
        public string Ip { get; set; }
        public double Power { get; set; }
        public double ElectricityConsumption { get; set; }
        public DateTime Date { get; set; }

        #endregion
    }
}
