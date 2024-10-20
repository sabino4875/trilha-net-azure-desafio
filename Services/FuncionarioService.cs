namespace TrilhaNetAzureDesafio.Services
{
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using TrilhaNetAzureDesafio.Models;
    using TrilhaNetAzureDesafio.Repositories;
    using TrilhaNetAzureDesafio.ViewModels;

    public class FuncionarioService : IFuncionarioService
    {
        private Boolean _disposable;
        private readonly IFuncionarioRepository _repository;
        private readonly IFuncionarioLogRepository _logRepository;
        private readonly IMapper _mapper;

        public FuncionarioService(IFuncionarioRepository repository, IFuncionarioLogRepository logRepository, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(repository, nameof(repository));
            ArgumentNullException.ThrowIfNull(repository, nameof(logRepository));
            ArgumentNullException.ThrowIfNull(repository, nameof(mapper));

            _repository = repository;
            _logRepository = logRepository;
            _mapper = mapper;
            _disposable = true;
        }

        public Boolean Delete(Guid id)
        {
            var exists = _repository.Count(e => e.Id == id) > 0;
            if (exists) 
            {
                var log = _repository.Find(e => e.Id == id);
                var result = _repository.Delete(e => e.Id == id);
                if(result)
                {
                    SaveExecutionLog(log, TipoAcao.Remocao);
                }
                return result;
            }
            return false;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (_disposable && disposing)
            {
                _disposable = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FuncionarioService()
        {
            Dispose(false);
        }

        public FuncionarioViewModel Find(Guid id)
        {
            var data = _repository.Find(e => e.Id == id);
            if(data!=null)
            {
                var result = _mapper.Map<FuncionarioViewModel>(data);
                return result;
            }
            return null;
        }

        public Guid Insert(AddFuncionarioViewModel entity)
        {
            var data = _mapper.Map<Funcionario>(entity);
            var result = _repository.Insert(data);
            if(result != Guid.Empty)
            {
                var log = _repository.Find(e => e.Id == result);
                SaveExecutionLog(log, TipoAcao.Inclusao);
            }
            return result;
        }

        public PagedViewModel<FuncionarioViewModel> List(String nome, Int32 page)
        {
            Expression<Func<Funcionario, Boolean>> filter = e => true;
            if(!String.IsNullOrEmpty(nome) && !String.IsNullOrWhiteSpace(nome))
            {
                filter = e => e.Nome == nome;
            }

            if (page < 1) page = 1;
            var limit = 30;
            var offset = (page - 1) * limit;

            var data = _repository.List(filter, limit, offset);
            var items = _mapper.Map<IEnumerable<FuncionarioViewModel>>(data);
            var count = _repository.Count(filter);
            return new PagedViewModel<FuncionarioViewModel>(count, limit, page, items);
        }

        public Boolean Exists(Guid id)
        {
            return _repository.Count(e => e.Id == id) > 0;
        }

        public Boolean Update(FuncionarioViewModel entity)
        {
            var exists = _repository.Count(e => e.Id == entity.Id) > 0;
            if (exists)
            {
                var data = _mapper.Map<Funcionario>(entity);
                var result = _repository.Update(data);
                if(result)
                {
                    SaveExecutionLog(data, TipoAcao.Atualizacao);
                }
                return result;
            }
            return false;
        }

        private void SaveExecutionLog(Funcionario entity, TipoAcao action)
        {
            var log = new FuncionarioLog(entity, action, action.ToString(), entity.Id.ToString());
            _logRepository.Insert(log);
        }

        public IEnumerable<FuncionarioLog> Log()
        {
            return _logRepository.List();
        }
    }
}
