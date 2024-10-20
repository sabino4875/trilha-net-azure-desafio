using System;
using System.Collections.Generic;
using TrilhaNetAzureDesafio.Models;

namespace TrilhaNetAzureDesafio.Repositories
{
    public interface IFuncionarioLogRepository : IDisposable
    {
        Boolean Insert(FuncionarioLog entity);
        IEnumerable<FuncionarioLog> List();
    }
}
