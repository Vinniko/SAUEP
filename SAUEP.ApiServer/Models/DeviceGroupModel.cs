using SAUEP.ApiServer.Interfaces;

namespace SAUEP.ApiServer.Models
{
    public sealed class DeviceGroupModel : IModel
    {
        #region Constructors

        public DeviceGroupModel(string title, int id = 0)
        {
            Id = id;
            Title = title;
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string Title { get; set; }

        #endregion
    }
}
