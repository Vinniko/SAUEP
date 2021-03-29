using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class RoleModel : IModel
    {
        #region Constructros
        public RoleModel()
        {

        }
        public RoleModel(string title, int id = 0)
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
