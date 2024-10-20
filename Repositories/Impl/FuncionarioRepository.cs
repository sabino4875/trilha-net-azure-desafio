namespace TrilhaNetAzureDesafio.Repositories.Impl
{
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using TrilhaNetAzureDesafio.Context;
    using TrilhaNetAzureDesafio.Models;

    public class FuncionarioRepository : IFuncionarioRepository
    {
        private Boolean _disposable;
        private readonly RHContext _context;
        private readonly ILogger _logger;

        public FuncionarioRepository(RHContext context) 
        { 
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            _context = context;
            _logger = Log.ForContext<FuncionarioRepository>();
            _disposable = true;
        }

        public Int32 Count(Expression<Func<Funcionario, Boolean>> criteria)
        {
            try
            {
                return _context.Funcionarios.Count(criteria);
            }
            catch (Exception ex) 
            {
                _logger.Error(ex, "Houve um erro ao tentar executar a contagem.");
                throw;
            }
        }

        public Boolean Delete(Expression<Func<Funcionario, Boolean>> criteria)
        {
            try
            {
                var entity = _context.Funcionarios.Where(criteria).FirstOrDefault();
                if (entity != null) 
                {
                    _context.Funcionarios.Remove(entity);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar excluir o registro.");
                throw;
            }
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

        ~FuncionarioRepository()
        {
            Dispose(false);
        }

        public Funcionario Find(Expression<Func<Funcionario, Boolean>> criteria)
        {
            try
            {
                var entity = _context.Funcionarios.Where(criteria).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar localizar o registro.");
                throw;
            }
        }

        public Guid Insert(Funcionario entity)
        {
            try
            {
                var result = _context.Funcionarios.Add(entity);
                _context.SaveChanges();
                return result.Entity.Id;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar incluir o registro.");
                throw;
            }
        }

        public IEnumerable<Funcionario> List(Expression<Func<Funcionario, 
                                         Boolean>> criteria, 
                                         Int32 limit, 
                                         Int32 offset
        )
        {
            try
            {
                var result = _context.Funcionarios.Where(criteria).Skip(offset).Take(limit);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar listar os registros.");
                throw;
            }
        }

        public Boolean Update(Funcionario entity)
        {
            try
            {
                var exists = _context.Funcionarios.Where(e => e.Id == entity.Id).Any();
                if (exists)
                {
                    _context.Funcionarios.Update(entity);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar atualizar o registro.");
                throw;
            }
        }
    }
}
