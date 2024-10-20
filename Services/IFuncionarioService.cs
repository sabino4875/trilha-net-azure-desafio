namespace TrilhaNetAzureDesafio.Services
{
    using System;
    using System.Collections.Generic;
    using TrilhaNetAzureDesafio.Models;
    using TrilhaNetAzureDesafio.ViewModels;

    public interface IFuncionarioService : IDisposable
    {
        Guid Insert(AddFuncionarioViewModel entity);
        Boolean Update(FuncionarioViewModel entity);
        Boolean Delete(Guid id);
        FuncionarioViewModel Find(Guid id);
        PagedViewModel<FuncionarioViewModel> List(String nome, Int32 page);
        Boolean Exists(Guid id);
        IEnumerable<FuncionarioLog> Log();
    }
}
