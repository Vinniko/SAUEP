using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Repositories;

namespace SAUEP.ApiServer.Services
{
    public class RegistrationService : IRegistration
    {
        #region Constructors

        public RegistrationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #endregion



        #region Main Logic

        public bool Registration(UserModel userModel)
        {
            foreach(var user in _userRepository.Get<UserModel>())
            {
                if(user.Login.Equals(userModel.Login))
                    return false;
            }
            _userRepository.Set(userModel);
            return true;
        }

        #endregion



        #region Fields

        private UserRepository _userRepository;

        #endregion
    }
}
