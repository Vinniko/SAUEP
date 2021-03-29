namespace SAUEP.Core.Interfaces
{
    public interface IValidation
    {
        bool Validate<T>(T data);
    }
}
