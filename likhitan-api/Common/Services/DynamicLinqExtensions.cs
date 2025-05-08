using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace likhitan_api.Common.Services
{
    public static class DynamicLinqExtensions
    {
        public static IQueryable<T> WhereEquals<T>(this IQueryable<T> source, string propertyPath, object? value)
        {
            if (string.IsNullOrWhiteSpace(propertyPath) || value == null)
                return source;

            var param = Expression.Parameter(typeof(T), "x");
            var property = GetNestedPropertyExpression(param, propertyPath);
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, Expression.Convert(constant, property.Type));
            var lambda = Expression.Lambda<Func<T, bool>>(equal, param);
            return source.Where(lambda);
        }

        public static IQueryable<T> WhereContains<T>(this IQueryable<T> source, string propertyPath, string keyword)
        {
            if (string.IsNullOrWhiteSpace(propertyPath) || string.IsNullOrWhiteSpace(keyword))
                return source;

            var param = Expression.Parameter(typeof(T), "x");
            var property = GetNestedPropertyExpression(param, propertyPath);

            if (property.Type != typeof(string)) return source;

            var method = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            var keywordExpr = Expression.Constant(keyword);
            var containsExpr = Expression.Call(property, method, keywordExpr);
            var lambda = Expression.Lambda<Func<T, bool>>(containsExpr, param);
            return source.Where(lambda);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyPath, bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
                return source;

            var param = Expression.Parameter(typeof(T), "x");
            var property = GetNestedPropertyExpression(param, propertyPath);
            var lambda = Expression.Lambda(property, param);

            var methodName = descending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, new object[] { source, lambda });

            return (IQueryable<T>)result!;
        }

        // Select Dynamic (Efficient with compiled accessors)
        private static readonly ConcurrentDictionary<string, Func<object, object?>> _compiledAccessors = new();

        public static List<IDictionary<string, object?>> SelectDynamic<T>(this IQueryable<T> source, params string[] propertyPaths)
        {
            var data = source.ToList(); // Only one ToList() to materialize once.

            var accessors = propertyPaths.ToDictionary(
                path => path,
                path => BuildAccessor<T>(path) // One compiled accessor per property
            );

            var result = new List<IDictionary<string, object?>>();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var path in propertyPaths)
                {
                    dict[path] = accessors[path](item);
                }
                result.Add(dict);
            }

            return result;
        }

        private static Func<object, object?> BuildAccessor<T>(string propertyPath)
        {
            string cacheKey = typeof(T).FullName + ":" + propertyPath;
            if (_compiledAccessors.TryGetValue(cacheKey, out var cached))
                return cached;

            var param = Expression.Parameter(typeof(object), "x");
            var casted = Expression.Convert(param, typeof(T));

            Expression body = casted;
            foreach (var prop in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, prop);
            }

            Expression final = Expression.Convert(body, typeof(object));
            var lambda = Expression.Lambda<Func<object, object?>>(final, param).Compile();

            _compiledAccessors[cacheKey] = lambda;
            return lambda;
        }

        // Get Nested Property Expression (Helper for dynamic property access)
        private static Expression GetNestedPropertyExpression(Expression param, string propertyPath)
        {
            Expression propertyExpr = param;
            foreach (var prop in propertyPath.Split('.'))
            {
                propertyExpr = Expression.PropertyOrField(propertyExpr, prop);
            }
            return propertyExpr;
        }

        public static IEnumerable<TResult> InnerJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector);
        }

        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner?, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, inners) => new { o, inners })
                .SelectMany(x => x.inners.DefaultIfEmpty(), (x, i) => resultSelector(x.o, i));
        }

        public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter?, TInner, TResult> resultSelector)
        {
            return inner
                .GroupJoin(outer, innerKeySelector, outerKeySelector, (i, outers) => new { i, outers })
                .SelectMany(x => x.outers.DefaultIfEmpty(), (x, o) => resultSelector(o, x.i));
        }

        public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter?, TInner?, TResult> resultSelector)
        {
            var left = outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, inners) => new { o, inners })
                .SelectMany(x => x.inners.DefaultIfEmpty(), (x, i) => new { x.o, i });

            var right = inner
                .GroupJoin(outer, innerKeySelector, outerKeySelector, (i, outers) => new { i, outers })
                .SelectMany(x => x.outers.DefaultIfEmpty(), (x, o) => new { o, i = x.i });

            var union = left.Concat(right)
                .GroupBy(x => new { OuterKey = outerKeySelector(x.o!), InnerKey = innerKeySelector(x.i!) })
                .Select(g => resultSelector(g.First().o, g.First().i));

            return union;
        }

        public static IEnumerable<TResult> OuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter?, TInner?, TResult> resultSelector)
        {
            return FullJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
        }

        public static IEnumerable<TResult> CrossJoin<TOuter, TInner, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.SelectMany(o => inner, (o, i) => resultSelector(o, i));
        }

        public static IEnumerable<TResult> SelfJoin<T, TResult>(
            this IEnumerable<T> source,
            Func<T, T, TResult> resultSelector)
        {
            return source.SelectMany(
                x => source,
                (x, y) => resultSelector(x, y)
            );
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (outerKeySelector == null) throw new ArgumentNullException(nameof(outerKeySelector));
            if (innerKeySelector == null) throw new ArgumentNullException(nameof(innerKeySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            var lookup = inner.ToLookup(innerKeySelector, comparer);

            foreach (var outerItem in outer)
            {
                var key = outerKeySelector(outerItem);
                var innerItems = lookup[key];
                yield return resultSelector(outerItem, innerItems);
            }
        }
    }

}
