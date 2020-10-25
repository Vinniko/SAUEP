using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    public sealed class DeviceModel : IModel
    {
        #region Constructors

        public DeviceModel(int id, int deviceGroupId, string serial, string title, string ip, string port, bool status, double maxPower, double minPower, double maxElecticityConsumption, double minElecticityConsumption)
        {
            Id = id;
            DeviceGroupId = deviceGroupId;
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
        public int DeviceGroupId { get; set; }
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
