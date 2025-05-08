using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace likhitan_api.Common.Repository
{
    public abstract class BaseRepository<T> where T : class
    {
        protected virtual Expression<Func<T, dynamic>>[] Includes => Array.Empty<Expression<Func<T, dynamic>>>();

        // Method that uses the includes
        protected IQueryable<T> GetQueryWithIncludes(dynamic _dbSet)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in Includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}
