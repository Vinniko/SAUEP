using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class DeviceModel : IModel
    {
        #region Constructors

        public DeviceModel()
        {

        }
        public DeviceModel(string deviceGroup, string serial, string title, string ip, string port, bool status, double maxPower, double minPower, double maxElecticityConsumption, double minElecticityConsumption, int id = 0)
        {
            Id = id;
            DeviceGroup = deviceGroup;
            Serial = serial;
            Title = title;
            Ip = ip;
            Port = port;
            Status = status;
            MaxPower = maxPower;
            MinPower = minPower;
            MaxElecticityConsumption = maxElecticityConsumption;
            MinElecticityConsumption = minElecticityConsumption;
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string DeviceGroup { get; set; }
        public string Serial { get; set; }
        public string Title { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public bool Status { get; set; }
        public double MaxPower { get; set; }
        public double MinPower { get; set; }
        public double MaxElecticityConsumption { get; set; }
        public double MinElecticityConsumption { get; set; }

        #endregion
    }
}
