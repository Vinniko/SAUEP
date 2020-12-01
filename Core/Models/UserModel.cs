using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class UserModel : IModel
    {
        #region Constructros

        public UserModel(string login, string password, string email, int id = 0, string role = "Пользователь")
        {
            Id = id;
            Login = login;
            Password = password;
            Email = email;
            Role = role;
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        #endregion
    }
}
