using SAUEP.Core.Interfaces;
using SAUEP.Core.Exceptions;

namespace SAUEP.Core.Services
{
    public sealed class LoginValidation : IValidation
    {
        #region Main Logic

        public bool Validate<T>(T data)
        {
            if (data as string == string.Empty)
                throw new LoginValidationException(_emptyErrorMessage);
            else if((data as string).Length < 4)
                throw new LoginValidationException(_lengthErrorMessage);
            else if((data as string).Contains(" "))
                throw new LoginValidationException(_spaceErrorMessage);
            else 
                return true;
        }

        #endregion



        #region Fields

        private const string _emptyErrorMessage = "Поле логина не может быть пустым";
        private const string _lengthErrorMessage = "Логин не может быть короче 4-х символов";
        private const string _spaceErrorMessage = "Логин не может содержать пробельные символы";

        #endregion
    }
}
