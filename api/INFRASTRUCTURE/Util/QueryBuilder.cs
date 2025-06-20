using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using INFRASTRUCTURE.ExceptionHandler;

namespace Infrastructure.Util;


public class QueryBuilder<TModel> where TModel : class
{
    /// <summary>
    /// Apply filters and relations to the query
    /// </summary>
    /// <param name="query">The query to apply the filters and relations to</param>
    /// <param name="filters">The filters to apply to the query, in the format of "field[operator]=value"</param>
    /// <param name="relations">The relations to apply to the query, separated by '|'</param>
    /// <returns>The query with the filters and relations applied</returns>
    public static IQueryable<TModel> Apply(IQueryable<TModel> query, Dictionary<string, string>? filters = null, string relations = "")
    {
        filters ??= [];
        relations ??= "";
        var relation = relations.Split('|');
        foreach (var rel in relation)
        {
            query = query.Include(rel);
        }
        foreach (var filter in filters)
        {
            var match = Regex.Match(filter.Key, @"^(?<path>[\w\.]+)\[(?<op>\w+)\]$");
            if (!match.Success) continue;
            string fld = match.Groups["field"].Value;
            string opt = match.Groups["op"].Value.ToLower();
            string val = filter.Value;
            query = query.Where(BuildExpression(fld, opt, val));
        }
        return query;
    }

    private static Expression<Func<TModel, bool>> BuildExpression(string path, string op, string value)
    {
        var param = Expression.Parameter(typeof(TModel), "x");

        Expression property = param;
        Type currentType = typeof(TModel);

        foreach (var member in path.Split('.'))
        {
            var propInfo = currentType.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new Error500Exception($"Property '{member}' not found on type '{currentType.Name}'");
            property = Expression.Property(property, propInfo);
            currentType = propInfo.PropertyType;
        }

        var typedValue = Convert.ChangeType(value, currentType);
        var constant = Expression.Constant(typedValue);

        Expression comparison = op switch
        {
            "eq"  => Expression.Equal(property, constant),
            "ne"  => Expression.NotEqual(property, constant),
            "gt"  => Expression.GreaterThan(property, constant),
            "gte" => Expression.GreaterThanOrEqual(property, constant),
            "lt"  => Expression.LessThan(property, constant),
            "lte" => Expression.LessThanOrEqual(property, constant),
            _     => throw new Error500Exception($"Unsupported operator '{op}'")
        };

        return Expression.Lambda<Func<TModel, bool>>(comparison, param);
    }
}
