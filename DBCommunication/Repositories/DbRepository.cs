using DBCommunication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Repositories
{
    internal class DbRepository : IDbRepository
    {
        private readonly DataContext _context;

        public DbRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        async Task<Guid> IDbRepository.Add<T>(T newEntity)
        {
            var entity = await _context.Set<T>().AddAsync(newEntity);
            return entity.Entity.Id;
        }

        async Task IDbRepository.AddRange<T>(IEnumerable<T> newEntities)
        {
            await _context.Set<T>().AddRangeAsync(newEntities);
        }

        IQueryable<T> IDbRepository.Get<T>(Expression<Func<T, bool>> selector)
        {
            return _context.Set<T>().Where(selector).AsQueryable();
        }

        IQueryable<T> IDbRepository.Get<T>()
        {
            return _context.Set<T>().AsQueryable();
        }

        IQueryable<T> IDbRepository.GetAll<T>()
        {
            return _context.Set<T>().AsQueryable();
        }

        async Task IDbRepository.Remove<T>(T entity)
        {
            await Task.Run(() => _context.Set<T>().Remove(entity));
        }

        async Task IDbRepository.RemoveRange<T>(IEnumerable<T> newEntities)
        {
            await Task.Run(() => _context.Set<T>().RemoveRange(newEntities));
        }

        async Task IDbRepository.Update<T>(T entity)
        {
            await Task.Run(() => _context.Set<T>().Update(entity));
        }

        async Task IDbRepository.UpdateRange<T>(IEnumerable<T> entities)
        {
            await Task.Run(() => _context.Set<T>().UpdateRange(entities));
        }
    }
}
