using SAUEP.Core.Interfaces;
using SAUEP.Core.Exceptions;

namespace SAUEP.Core.Services
{
    public sealed class PasswordValidation : IValidation
    {
        #region Main Logic

        public bool Validate<T>(T data)
        {
            if (data as string == string.Empty)
                throw new PasswordValidationException(_emptyErrorMessage);
            else if ((data as string).Length < 8)
                throw new PasswordValidationException(_lengthErrorMessage);
            else if ((data as string).Contains(" "))
                throw new PasswordValidationException(_spaceErrorMessage);
            else
                return true;
        }

        #endregion



        #region Fields

        private const string _emptyErrorMessage = "Поле пароля не может быть пустым";
        private const string _lengthErrorMessage = "Пароль не может быть короче 8-ми символов";
        private const string _spaceErrorMessage = "Пароль не может содержать пробельные символы";

        #endregion
    }
}
