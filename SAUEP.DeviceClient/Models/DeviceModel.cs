using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    public sealed class DeviceModel : IModel
    {
        #region Constructors

        public DeviceModel(int id, string deviceGroup, string serial, string title, string ip, string port, bool status, double maxPower, double minPower)
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

        #endregion
    }
}
