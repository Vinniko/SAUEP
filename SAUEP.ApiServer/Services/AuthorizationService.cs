using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Repositories;
using System.Security.Claims;
using System.Collections.Generic;


namespace SAUEP.ApiServer.Services
{
    public sealed class AuthorizationService : IAuthorization
    {
        #region Constructors

        public AuthorizationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #endregion



        #region Main Logic

        public ClaimsIdentity Authorize(string username, string password)
        {
            var users = _userRepository.Get<UserModel>();
            UserModel user = null;
            for (var i = 0; i < users.Count; i++)
            {
                if ((users as List<UserModel>)[i].Login.Equals(username) && (users as List<UserModel>)[i].Password.Equals(password))
                    user = (users as List<UserModel>)[i];
            }
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }

        #endregion



        #region Fields

        private UserRepository _userRepository;

        #endregion
    }
}
