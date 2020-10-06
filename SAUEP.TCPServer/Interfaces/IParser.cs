namespace Core.Interfaces
{
    interface IParser
    {
        T Pars<T>(string json);
    }
}
