﻿namespace SAUEP.ApiServer.Interfaces
{
    public interface IParser
    {
        T Pars<T>(string data);
        string Depars<T>(T data);
    }
}
