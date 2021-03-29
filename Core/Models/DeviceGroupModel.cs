using System.Collections.ObjectModel;
using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class DeviceGroupModel : IModel
    {
        #region Constructors

        public DeviceGroupModel()
        {
            DeviceModels = new ObservableCollection<DeviceModel>();
        }
        public DeviceGroupModel(string title, int id = 0)
        {
            Id = id;
            Title = title;
            DeviceModels = new ObservableCollection<DeviceModel>();
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<DeviceModel> DeviceModels {get; set;}

        #endregion
    }
}
